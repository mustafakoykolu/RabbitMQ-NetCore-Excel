using ExcelWorkerService;
using ExcelWorkerService.Models;
using ExcelWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddDbContext<ExcelCreatorNewDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("mssql"));
}, ServiceLifetime.Singleton);
var rabbitConn = builder.Configuration.GetConnectionString("rabbitMQ");
builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(builder.Configuration.GetConnectionString("rabbitMQ")) });
builder.Services.AddSingleton<RabbitMQClientService>();

var host = builder.Build();
host.Run();
