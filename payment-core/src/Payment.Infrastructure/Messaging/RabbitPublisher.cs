using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Payment.Infrastructure.Messaging
{
    public class RabbitPublisher
    {

        private readonly IConnection _connection;

        public RabbitPublisher(IConnection connection)
        {
            _connection = connection;
        }

        public void Publish(object message)
        {

            var channel = _connection.CreateModel();

            channel.QueueDeclare(
                "payments",
                true,
                false,
                false);

            var body = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(message));

            channel.BasicPublish(
                "",
                "payments",
                null,
                body);

        }
    }
}