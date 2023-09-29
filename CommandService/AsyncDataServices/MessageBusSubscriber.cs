using System.Text;
using System.Threading.Channels;
using CommandService.EventProcessors;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataService
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private ConnectionFactory? _connectionFactory;
        private IConnection? _connection;
        private IModel? _channel;
        private string? _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;

            InitializeRabbitMQ();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) => {
                Console.WriteLine("--> Event Received!");

                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);

                _eventProcessor.ProcessEvent(message);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        private void InitializeRabbitMQ() {
            _connectionFactory = new() {
                HostName = _configuration["RabbitMQHost"],
                Port = int.TryParse(_configuration["RabbitMQPort"], out int port) ? port : 0
            };
            
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");

            Console.WriteLine("--> Listening on the Message Bus...");

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection Shutdown");
        }

        public override void Dispose()
        {

            if(_channel != null && _channel.IsOpen) {
                _channel?.Close();
                _connection?.Close();
            }

            base.Dispose();
        }
    }
}