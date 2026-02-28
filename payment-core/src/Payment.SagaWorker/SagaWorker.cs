using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Payment.SagaWorker
{
    public class SagaWorker : BackgroundService
    {

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                await Task.Delay(5000);

            }

        }

    }
}