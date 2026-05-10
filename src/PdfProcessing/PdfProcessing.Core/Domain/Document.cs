namespace PdfProcessing.Domain;

/// <summary> to be processed
/// </summary>
public record Document : DomainBase
{
    public string DisplayName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DocumentState State { get; set; } = DocumentState.Unknown;
}
