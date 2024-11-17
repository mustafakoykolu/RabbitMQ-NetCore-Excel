using ClosedXML.Excel;
using ExcelWorkerService.Models;
using ExcelWorkerService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ExcelWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private RabbitMQClientService _rabbitMQClientService;
        private readonly ExcelCreatorNewDbContext _excelCreatorNewDbContext;
        private IChannel _channel;
        public Worker(ILogger<Worker> logger,ExcelCreatorNewDbContext excelCreatorNewDbContext,RabbitMQClientService rabbitMQClientService)
        {
            _logger = logger;
            _excelCreatorNewDbContext = excelCreatorNewDbContext;
            _rabbitMQClientService = rabbitMQClientService;
        }
        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = await _rabbitMQClientService.Connect();
            await _channel.BasicQosAsync(0, 1, false);
            await base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            await _channel.BasicConsumeAsync(RabbitMQClientService.QueName, false, consumer);
            consumer.ReceivedAsync += ConsumerRecieved;

        }
        private async Task ConsumerRecieved(object sender, BasicDeliverEventArgs @event)
        {
            await Task.Delay(5000);

            var createExcelMessage = JsonSerializer.Deserialize<CreateExcelMessage>(Encoding.UTF8.GetString(@event.Body.ToArray()));

            using var ms = new MemoryStream();
            var wb = new XLWorkbook();
            var ds = new DataSet();
            ds.Tables.Add(GetDataTable("userFile"));
            wb.Worksheets.Add(ds);
            wb.SaveAs(ms);

            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(ms.ToArray()),"excelFile",Guid.NewGuid().ToString()+".xlsx");
            var baseUrl = "http://localhost:5293" + "/api/files";
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));

                var response = await httpClient.PostAsync(baseUrl+$"?fileId={createExcelMessage.FileId}", content);
                if (response.IsSuccessStatusCode) {
                    _logger.LogInformation("File succesfully created");
                    await _channel.BasicAckAsync(@event.DeliveryTag,true);
                }
            }
        }
        private DataTable GetDataTable(string tableName)
        {
            List<UserFile> files;
            files= _excelCreatorNewDbContext.UserFiles.ToList();

            DataTable dataTable = new DataTable() { TableName = tableName};

            dataTable.Columns.Add("UserId", typeof(string));
            foreach (var item in files)
            {
                dataTable.Rows.Add(item.Id);
                
            }
            return dataTable;
        }
    }
}
