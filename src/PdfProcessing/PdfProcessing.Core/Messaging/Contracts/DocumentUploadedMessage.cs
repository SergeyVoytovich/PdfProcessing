namespace PdfProcessing.Messaging.Contracts;

public record DocumentUploadedMessage
{
    public Guid DocumentId { get; set; }
}
