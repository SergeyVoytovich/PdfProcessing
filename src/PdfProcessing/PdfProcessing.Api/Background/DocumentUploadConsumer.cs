using PdfProcessing.Application;
using PdfProcessing.Messaging;
using PdfProcessing.Messaging.Contracts;

namespace PdfProcessing.Api.Background;

public class DocumentUploadConsumer(IMessageBus messageBus, IServiceScopeFactory serviceFactory) : BackgroundService
{
    public virtual IMessageBus MessageBus { get; } = messageBus;
    public virtual IServiceScopeFactory ServiceFactory { get; } = serviceFactory;


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await MessageBus.SubscribeAsync<DocumentUploadedMessage>( HandleDocumentUploadedMessageAsync, stoppingToken);
    }

    protected virtual async Task HandleDocumentUploadedMessageAsync(DocumentUploadedMessage message, CancellationToken cancellationToken)
    {
        using var scope = ServiceFactory.CreateScope();
        var application = scope.ServiceProvider.GetRequiredService<IApplication>();
        await application.Documents.ProcessAsync(message.DocumentId, cancellationToken);
    }
}
