using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;

namespace ExcelCreator.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IChannel _channel;
        private readonly ILogger<RabbitMQClientService> _logger;

        public static string ExchangeName = "ExcelDirectExchange";
        public static string RouitingExcel = "excel-route-file";
        public static string QueName = "queue-excel-file";


        public RabbitMQClientService(ConnectionFactory connectionFactory,ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<IChannel> Connect()
        {
            _connection = await _connectionFactory.CreateConnectionAsync();
            if (_channel is { IsOpen: true })
            {
                return _channel;
            }
            _channel= await _connection.CreateChannelAsync();
            _channel.ExchangeDeclareAsync(ExchangeName,type:"direct",true,false).Wait();
            _channel.QueueDeclareAsync(QueName, true, false, false, null).Wait();
            _channel.QueueBindAsync(exchange: ExchangeName, queue: QueName, routingKey: RouitingExcel).Wait();

            _logger.LogInformation("oluşturuldu");

            return _channel;
    
        }
       

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
