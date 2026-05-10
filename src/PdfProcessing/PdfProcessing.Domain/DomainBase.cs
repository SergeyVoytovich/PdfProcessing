namespace PdfProcessing.Domain;

/// <summary>
/// Base domain entity
/// </summary>
public record DomainBase
{
    
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public string CreateBy { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;

    public DateTime DeletedAt { get; set; }
    public string DeletedBy { get; set; } = string.Empty;   
}
