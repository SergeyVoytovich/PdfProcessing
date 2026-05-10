namespace PdfProcessing.Domain;

/// <summary>
/// Base domain entity
/// </summary>
public record DomainBase
{
    public Guid Id { get; set; }
}
