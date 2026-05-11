using Microsoft.Extensions.Logging;
using PdfProcessing.Messaging.Contracts;
using PdfProcessing.Messaging.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PdfProcessing.Messaging;

internal class RabbitMqBus : IMessageBus, IAsyncDisposable
{
    protected virtual IConnection Connection { get; }
    protected virtual ILogger<RabbitMqBus> Logger { get; }
    protected virtual IChannel Channel { get; }

    public RabbitMqBus(IConnection connection, ILogger<RabbitMqBus> logger)
    {
        Connection = connection;
        Logger = logger;

        Logger.LogTrace("Start up RabbitMqBus");
        Channel = StartUpAsync().GetAwaiter().GetResult(); //todo fix it, move to factory
    }

    private async Task<IChannel> StartUpAsync()
    {
        var channel = await Connection.CreateChannelAsync();
        await channel.QueueDeclareAsync<DocumentUploadedMessage>();
        return channel;
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : IMessage
    {
        await Channel.PublishMessageAsync(message, cancellationToken);
        Logger.LogInformation($"Message of type {typeof(T).Name} published");
    }

    public async Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellation = default) where T : IMessage
    {
        var consumer = new AsyncEventingBasicConsumer(Channel);

        Logger.LogTrace($"Subscribed to messages of type {typeof(T).Name}");

        consumer.ReceivedAsync += async (_, args) =>
        {
            var message = args.Body.ToMessage<T>();
            if (message is not null)
            {
                await handler(message, cancellation);
                Logger.LogTrace($"Message of type {typeof(T).Name} processed");
            }

            await Channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken: cancellation);
        };

        await Channel.ConsumeMessageAsync<T>(consumer);
    }


    public async ValueTask DisposeAsync()
    {
        Logger.LogTrace("Disposing RabbitMQ bus");
        await Channel.DisposeAsync();
        await Connection.DisposeAsync();
    }
}
