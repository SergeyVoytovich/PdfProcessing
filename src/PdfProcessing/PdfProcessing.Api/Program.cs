using PdfProcessing.Api.Background;
using PdfProcessing.Api.Configuration;
using PdfProcessing.Application;
using PdfProcessing.Data;
using PdfProcessing.Messaging;
using Serilog;

// Serilog (for prodation need to pass section from application.json)
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Custom services
builder.Services.AddPdfProcessingApplication();
builder.Services.AddPdfProcessingDataToNpsql(builder.Configuration.PdfStorage());
builder.Services.AddPdfProcessingMessaging(builder.Configuration.MessagingConfiguration());
builder.Services.AddHostedService<DocumentUploadConsumer>();

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Serilog
builder.Host.UseSerilog();

try
{
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
  await Log.CloseAndFlushAsync();
}
