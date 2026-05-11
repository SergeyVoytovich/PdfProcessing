using PdfProcessing.Messaging.Contracts;
using PdfProcessing.Messaging.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PdfProcessing.Messaging;

internal class RabbitMqBus : IMessageBus, IAsyncDisposable
{
    protected virtual IConnection Connection { get; }
    protected virtual IChannel Channel { get; }

    public RabbitMqBus(IConnection connection)
    {
        Connection = connection;
        Channel = StartUpAsync().GetAwaiter().GetResult(); //todo fix it
    }

    private async Task<IChannel> StartUpAsync()
    {
        var channel = await Connection.CreateChannelAsync();
        await channel.QueueDeclareAsync<DocumentUploadedMessage>();
        return channel;
    }

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : IMessage
        => Channel.PublishMessageAsync(message, cancellationToken);

    public async Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellation = default) where T : IMessage
    {
        var consumer = new AsyncEventingBasicConsumer(Channel);

        consumer.ReceivedAsync += async (_, args) =>
        {
            var message = args.Body.ToMessage<T>();
            if (message is not null)
            {
                await handler(message, cancellation);
            }

            await Channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken: cancellation);
        };

        await Channel.ConsumeMessageAsync<T>(consumer);
    }


    public async ValueTask DisposeAsync()
    {
        await Channel.DisposeAsync();
        await Connection.DisposeAsync();
    }
}
