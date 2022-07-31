using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace Movies.Client.Handlers
{
    public class TimeOutDelegatingHandler : DelegatingHandler
    {
        private readonly TimeSpan timeOut = TimeSpan.FromSeconds(100);

        public TimeOutDelegatingHandler(TimeSpan time) : base()
        {
            timeOut = time;
        }

        public TimeOutDelegatingHandler(HttpMessageHandler innerHandler, TimeSpan time) : base(innerHandler)
        {
            timeOut = time;
        }

        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                linkedCancellationTokenSource.CancelAfter(timeOut);

                try
                {
                    return await base.SendAsync(request, linkedCancellationTokenSource.Token);
                }
                catch (OperationCanceledException oc)
                {
                    if (!cancellationToken.IsCancellationRequested) throw new TimeoutException("Request timed out.", oc);
                    throw;
                }
            }
        }
    }
}
