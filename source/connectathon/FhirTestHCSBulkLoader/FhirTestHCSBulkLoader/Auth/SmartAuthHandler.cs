using CDC.DEX.FHIR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using static FhirTestHCSBulkLoader.Config.LogConfig;


namespace FhirTestHCSBulkLoader.Auth
{
    internal class SmartAuthHandler : IAuthHandler
    {
        private const string COMPLETE_CERT_FILENAME = "complete_certificate.pfx";
        private const string CERT_FILENAME = "certificate.cer";
        private const string PRIVATEKEY_FILENAME = "private_key.pem";
        private const string PUBLICKEY_FILENAME = "public_key.pem";


        public async Task<string> GetFhirServerToken(IConfiguration config, HttpClient httpClient)
        {


            string bearerToken = "";


            // Cert Stuff
            string certPath = $"{FhirTestHCSBulkLoaderMain.config["RootPath"]}\\_Certificates";

            string[] files = Directory.GetFiles(certPath);

            // clean this up
            bool completeCertFilePresent, certFilePresent, privateKeyPemPresent, publicKeyPemPresent;
            completeCertFilePresent = certFilePresent = privateKeyPemPresent = publicKeyPemPresent = false;

            bool generateCert = true;

            X509Certificate2 cert = null;

            if (files.Length > 0)
            {
                Log($"Existing files found in {certPath}", LogType.Info);
                foreach (string file in files)
                {
                    string filename = Path.GetFileName(file);
                    if (filename == COMPLETE_CERT_FILENAME) completeCertFilePresent = true;
                    if (filename == CERT_FILENAME) certFilePresent = true;
                    if (filename == PUBLICKEY_FILENAME) publicKeyPemPresent = true;
                    if (filename == PRIVATEKEY_FILENAME) privateKeyPemPresent = true;
                }
                if (completeCertFilePresent && certFilePresent && privateKeyPemPresent && publicKeyPemPresent)
                {
                    Log($"All cert files found in {certPath}, using them", LogType.Info);

                    // Import the existing cert
                    //Byte[] pfxBytes = File.ReadAllBytes($"{certPath}\\{COMPLETE_CERT_FILENAME}");
                    cert = new X509Certificate2($"{certPath}\\{COMPLETE_CERT_FILENAME}");
                    generateCert = false;
                }
            }
            if (generateCert)
            {
                //Generate certs because they aren't in the cert folder
                cert = GenerateSelfSignedCertificate(config["TokenSubject"], DateTime.Now.AddDays(90));
                ExportCertificate(cert, certPath);
            }

            string generatedJwt = GenerateJwtUsingCert(cert,
                config["TokenAudience"],
                config["TokenIssuer"],
                config["TokenExpiry"],
                config["TokenSubject"]);

            // TODO: THIS IS FOR DEBUG ONLY
            Log($"Generated JWT token succesfully: {generatedJwt}", LogType.Good);


            var dict = new Dictionary<string, string>();
            dict.Add("grant_type", "Client_Credentials");
            dict.Add("client_id", config["ClientId"]);
            dict.Add("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
            dict.Add("client_assertion", generatedJwt);

            //using (var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"{config["BaseFhirUrl"]}/basicAuth") { Content = new FormUrlEncodedContent(dict) })
            using (var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"https://login.microsoftonline.com/9ce70869-60db-44fd-abe8-d2767077fc8f/oauth2/token") { Content = new FormUrlEncodedContent(dict) })
            {
                Log("SMART Authenticating with Fhir Server", LogType.Info);

                var tokenResponse = await httpClient.SendAsync(tokenRequest);

                var result = await tokenResponse.Content.ReadFromJsonAsync<AuthTokenResult>();

                if (!string.IsNullOrEmpty(result!.access_token))
                {
                    Log("Access Token Generated Successfully", LogType.Info);
                }
                else
                {
                    Log("Access Token failed to be retrieved", LogType.Bad);
                }

                bearerToken = result!.access_token;
            }

            return bearerToken;

        }

        private void ExportCertificate(X509Certificate2 cert, string path)
        {
            //string certPath = $"{path}\\_Certificates";



            byte[] certBytes = cert.Export(X509ContentType.Pfx);
            byte[] certificateBytes = cert.RawData;
            char[] certificatePem = PemEncoding.Write("CERTIFICATE", certificateBytes);

            AsymmetricAlgorithm key = cert.GetRSAPrivateKey();
            byte[] pubKeyBytes = key.ExportSubjectPublicKeyInfo();
            byte[] encryptedPrivKeyBytes = key.ExportEncryptedPkcs8PrivateKey("", new PbeParameters(
                    PbeEncryptionAlgorithm.Aes256Cbc,
                    HashAlgorithmName.SHA256,
                    iterationCount: 100_000));

            char[] pubKeyPem = PemEncoding.Write("PUBLIC KEY", pubKeyBytes);
            char[] privKeyPem = PemEncoding.Write("PRIVATE KEY", encryptedPrivKeyBytes);

            Log("Cert Files Generated Succesfully", LogType.Good);

            Directory.CreateDirectory(path);

            File.WriteAllBytes($"{path}\\{COMPLETE_CERT_FILENAME}", certBytes);
            File.WriteAllText($"{path}\\{CERT_FILENAME}", new String(certificatePem));
            File.WriteAllText($"{path}\\{PRIVATEKEY_FILENAME}", new String(privKeyPem));
            File.WriteAllText($"{path}\\{PUBLICKEY_FILENAME}", new String(pubKeyPem));

        }

        public static string GenerateJwtUsingCert(X509Certificate2 certificate, string audience, string issuer, string expiryMinutes, string subject)
        {
            var securityKey = new X509SecurityKey(certificate);
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha384);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, subject),
                new Claim(JwtRegisteredClaimNames.Aud, audience),
                new Claim(JwtRegisteredClaimNames.Iss, issuer),
                new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(int.Parse(expiryMinutes)).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, "testanyuniquevaluehere"),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials
            );
            //token.Header.Add("jku", securityKey.KeyId);
            token.Header.Add("x5t", securityKey.X5t);

            var jwtHandler = new JwtSecurityTokenHandler();

            string generatedJWT = jwtHandler.WriteToken(token);

            return generatedJWT;
        }

        public static X509Certificate2 GenerateSelfSignedCertificate(string subjectName, DateTime expirationDate)
        {
            var rsa = RSA.Create(2048); // Generate a 2048-bit RSA key pair

            var request = new CertificateRequest($"CN={subjectName}", rsa, HashAlgorithmName.SHA384, RSASignaturePadding.Pkcs1);

            var notBefore = DateTime.UtcNow.AddDays(-1);
            var notAfter = expirationDate.ToUniversalTime();

            var certificate = request.CreateSelfSigned(notBefore, notAfter);

            return new X509Certificate2(certificate.Export(X509ContentType.Pfx), "", X509KeyStorageFlags.Exportable);
        }




    }
}
