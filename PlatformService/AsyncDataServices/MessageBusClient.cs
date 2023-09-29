using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsycnDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel? _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionFactory connectionFactory = new ConnectionFactory() {
                HostName = configuration["RabbitMQHost"],
                Port = int.TryParse(configuration["RabbitMQPort"], out int port) ? port : 0
            };

            try {
                _connection = connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to Message Bus");

            } catch(Exception ex) {
                Console.WriteLine($"--> Could not connect to Message Bus: {ex.Message}");
            }
        }

        public void PublishNewPlatform(PlatformPublishDto platformPublishDto)
        {
            String message = JsonSerializer.Serialize(platformPublishDto);

            if(_connection.IsOpen) {
                Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            } else {
                Console.WriteLine("--> RabbitMQ Connection Closed, not sending");
            }

        }

        private void SendMessage(string message) {
            byte[] body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "trigger",
                routingKey: "",
                basicProperties: null,
                body: body);
            Console.WriteLine($"--> We have sent {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("Message Bus Disposed");
            
            if(_channel.IsOpen) {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }
    }

}