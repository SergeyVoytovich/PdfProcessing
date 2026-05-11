namespace PdfProcessing.Messaging;

internal interface IMessageBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellation = default);
    Task Subscribe<T>(Func<T, CancellationToken, Task> handler);
}
