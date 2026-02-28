using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Payment.Infrastructure.Data;
using Payment.Infrastructure.Messaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Payment.OutboxWorker
{
    public class Worker : BackgroundService
    {

        private readonly PaymentDbContext _db;

        private readonly RabbitPublisher _publisher;

        public Worker(
            PaymentDbContext db,
            RabbitPublisher publisher)
        {
            _db = db;
            _publisher = publisher;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                var messages = await _db.OutboxMessages
                    .Where(x => !x.Processed)
                    .ToListAsync();

                foreach (var msg in messages)
                {

                    _publisher.Publish(msg.Payload);

                    msg.Processed = true;

                }

                await _db.SaveChangesAsync();

                await Task.Delay(2000);

            }

        }

    }
}