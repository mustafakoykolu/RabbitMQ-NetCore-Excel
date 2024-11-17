using ExcelCreator.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ExcelCreator.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitmqClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitmqClientService)
        {
            _rabbitmqClientService = rabbitmqClientService;
        }
        public async void Publish(CreateExcelMessage excelMessage)
        {
            var channel = await _rabbitmqClientService.Connect();

            var bodyString = JsonSerializer.Serialize(excelMessage);

            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            await channel.BasicPublishAsync(exchange: RabbitMQClientService.ExchangeName, routingKey: RabbitMQClientService.RouitingExcel, body: bodyByte);
        }
    }
}
