namespace PdfProcessing.Domain;

/// <summary>
/// Represents the content associated with a document
/// </summary>
public record DocumentContent : DomainBase
{
    public Guid DocumentId { get; set; }
    public int PageNumber { get; set; }
    public string Content { get; set; } = string.Empty;
}