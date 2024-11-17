using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorkerService.Services
{
    public class RabbitMQClientService
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IChannel _channel;
        private readonly ILogger<RabbitMQClientService> _logger;

        public static string ExchangeName = "ExcelDirectExchange";
        public static string RouitingExcel = "excel-route-file";
        public static string QueName = "queue-excel-file";


        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
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
            _channel = await _connection.CreateChannelAsync();
           
            _logger.LogInformation("queue connection succesfull");

            return _channel;

        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
