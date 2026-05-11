using Microsoft.Extensions.DependencyInjection;
using PdfProcessing.Messaging.Configuration;
using RabbitMQ.Client;

namespace PdfProcessing.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPdfProcessingMessaging(this IServiceCollection services, MessageBusConfiguration configuration)
    {
        services.AddSingleton(_ => 
        {
            var factory = new ConnectionFactory() 
            {
                HostName = configuration.Host,
                UserName = configuration.Username,
                Password = configuration.Password,
            };
            return factory.CreateConnectionAsync().GetAwaiter().GetResult();


        });
        services.AddSingleton<IMessageBus, RabbitMqBus>();
        return services;
    }
}
