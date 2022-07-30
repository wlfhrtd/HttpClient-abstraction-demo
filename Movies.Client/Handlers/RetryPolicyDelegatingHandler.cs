using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace Movies.Client.Handlers
{
    public class RetryPolicyDelegatingHandler : DelegatingHandler
    {
        private readonly int maximumAmountOfRetries = 3;

        public RetryPolicyDelegatingHandler(int maxRetries) : base()
        {
            maximumAmountOfRetries = maxRetries;
        }

        public RetryPolicyDelegatingHandler(HttpMessageHandler innerHandler, int maxRetries) : base(innerHandler)
        {
            maximumAmountOfRetries = maxRetries;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            for (int i = 0; i < maximumAmountOfRetries; i++)
            {
                response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode) return response;
            }

            return response;
        }
    }
}
