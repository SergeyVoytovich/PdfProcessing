using PdfProcessing.Api.Background;
using PdfProcessing.Api.Configuration;
using PdfProcessing.Application;
using PdfProcessing.Data;
using PdfProcessing.Messaging;

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
