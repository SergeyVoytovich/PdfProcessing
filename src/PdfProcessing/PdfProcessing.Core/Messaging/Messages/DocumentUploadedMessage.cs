using PdfProcessing.Messaging.Messages;

namespace PdfProcessing.Messaging.Contracts;

public record DocumentUploadedMessage : IMessage
{
    public Guid DocumentId { get; set; }
}
