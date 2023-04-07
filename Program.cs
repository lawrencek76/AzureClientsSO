using Azure.Data.Tables;
using Microsoft.Extensions.Azure;
using Serilog;

// Ensure Azurite is running to run demo
// npm install -g azurite
// azurite --silent

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .WriteTo.Console()
    .WriteTo.AzureTableStorage(services.GetRequiredService<TableServiceClient>(), storageTableName: "test"));

//this causes a stack overflow / infinite loop in di
builder.Services.AddAzureClients(clients =>
{
    _ = clients.AddTableServiceClient("UseDevelopmentStorage=true");
});

// overwriting the registration works as expected
// builder.Services.AddSingleton(new TableServiceClient("UseDevelopmentStorage=true"));

WebApplication app = builder.Build();

app.Map("/", (TableServiceClient serviceClient) =>
{
    TableClient client = serviceClient.GetTableClient("test");
    _ = serviceClient.CreateTableIfNotExists("test");
    return "Hello World";
});

app.Run();
