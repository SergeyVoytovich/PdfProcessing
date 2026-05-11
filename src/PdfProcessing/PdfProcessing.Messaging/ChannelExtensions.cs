using PdfProcessing.Messaging.Messages;
using RabbitMQ.Client;

namespace PdfProcessing.Messaging;

internal static class ChannelExtensions
{
    public static Task QueueDeclareAsync<T>(this IChannel channel, CancellationToken cancellationToken = default)
        where T : IMessage
        => channel.QueueDeclareAsync(
               queue: typeof(T).Name,
               durable: true,
               exclusive: false,
               autoDelete: false,
               cancellationToken: cancellationToken);

    public static async Task PublishMessageAsync<T>(this IChannel channel, T message, CancellationToken cancellationToken = default)
        where T : IMessage
        => await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: typeof(T).Name,
            mandatory: false,
            basicProperties: new BasicProperties(),
            body: message.ToBytes(),
            cancellationToken: cancellationToken);

    public static async Task ConsumeMessageAsync<T>(this IChannel channel, IAsyncBasicConsumer consumer, CancellationToken cancellationToken = default) 
        where T : IMessage
        => await channel.BasicConsumeAsync(
            queue: typeof(T).Name,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);
}
