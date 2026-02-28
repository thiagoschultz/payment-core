using Polly;
using Polly.CircuitBreaker;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace payment_core.src.Payment.Infrastructure.Http
{
    public class PaymentHttpClient
    {

        private readonly HttpClient _client;

        private readonly AsyncCircuitBreakerPolicy _policy;

        public PaymentHttpClient(HttpClient client)
        {

            _client = client;

            _policy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    3,
                    TimeSpan.FromSeconds(30));

        }

        public async Task<string> SendAsync()
        {

            return await _policy.ExecuteAsync(async () =>
            {

                return await _client.GetStringAsync("http://pix/api");

            });

        }
    }
}