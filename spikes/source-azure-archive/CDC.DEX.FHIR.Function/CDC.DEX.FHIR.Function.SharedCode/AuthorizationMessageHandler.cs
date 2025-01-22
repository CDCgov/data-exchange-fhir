using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDC.DEX.FHIR.Function.SharedCode
{
    public class AuthorizationMessageHandler : HttpClientHandler
    {
        public System.Net.Http.Headers.AuthenticationHeaderValue Authorization { get; set; }
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Authorization != null)
                request.Headers.Authorization = Authorization;
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
