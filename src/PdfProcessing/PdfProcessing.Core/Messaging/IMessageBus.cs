using PdfProcessing.Messaging.Messages;

namespace PdfProcessing.Messaging;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellation = default) where T : IMessage;
    Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellation = default) where T : IMessage;
}
