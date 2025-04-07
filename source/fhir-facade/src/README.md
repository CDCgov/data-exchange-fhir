****CDC FHIR Façade Repository**
**Overview**
This repository contains the codebase for the CDC FHIR Façade solution. The purpose of the Façade is to ingest and store FHIR data while providing additional functionality for authentication, authorization, and observability. This proof-of-concept implementation is designed to run on AWS cloud infrastructure.

****Running the Application Locally with AWS****
**Prerequisites
Before running the application locally with AWS, ensure you have the following setup in your AWS account:
- S3 Bucket with write access for:
    - FHIR Bundles
    - Logs
    - OpenTelemetry data
- Log Group for writing logs
- IAM User with appropriate permissions to read/write to the S3 Bucket and Log Group
- [AWS Setup Instructions](https://docs.aws.amazon.com/AmazonS3/latest/userguide/walkthrough1.html)

When running locally with AWS Bundles, logs, and telemetry will be saved to the configured AWS services.

**Configuration Steps**
1. Update appsettings.Local.json:
    a) Set your AWS credentials: AccessKey and SecretKey
    b) Set "RunEnvironment": "AWS"
    c) Set "VerifyAuthURL" using the Token Signing Key URL from:
        AWS Console → Amazon Cognito → User Pools → [Your Pool] → Token  Signing Key URL

2. Set ASP.NET Core Environment:
In Properties/launchSettings.json, update the "environmentVariables" section in all three profiles to: "ASPNETCORE_ENVIRONMENT": "Local"

3. Run the Application:
- Visual Studio: Click the green "Start" button or press F5
- Command Line:
      cd path/to/project
      dotnet run
  
4. Authenticate API Requests:
- In Swagger, go to POST /Auth and set:
- client_id and client_secret from AWS Cognito:
    - AWS Console → Cognito → User Pools → [Your Pool] → App Clients
- Send the request and copy the access_token from the response.
- In POST /Bundle OK, choose Bearer Token under Authorization and paste the access token.

5. FHIR Bundle can be send using two different formats:
- Raw JSON: Paste directly in Body → Raw
- File Upload: Use Body → form-data → Key: File, Type: File, Value: [Select file]

6. Send POST Request:
POST http://localhost:5215/Bundle

⚠️ File size must not exceed 300MB

****Running Locally Without AWS****
When not using AWS, FHIR Bundles are saved to the local file system, and logs are printed to the console.

**Configuration Steps**
1. Update appsettings.Local.json:
- "RunEnvironment": "Local",
- "FileSettings": {
  "LocalDevFolder": "C:\\Path\\To\\Save\\Bundles"
  }
  
2. Modify Project File:
- In OneCDPFHIRFacade.csproj, set:
<DefineConstants>runLocal</DefineConstants>

3. Run the Application:
- Visual Studio: Click the green "Start" button or press F5
- Command Line:
    cd path/to/project
    dotnet run
  
4. Send FHIR Bundle:
Supported formats:
- Raw JSON: Body -> raw -> Paste in Body
- File Upload: Body → form-data → Key: File, Type: File, Value: [Select file]

5. Send POST Request:
- POST http://localhost:5215/Bundle

⚠️ File size must not exceed 300MB

****Docker Instructions****
Build Docker Image
docker build -t one-cdp-fhir-facade .

Run Docker Container
docker run -p 8080:8080 -p 80:80 one-cdp-fhir-facade
