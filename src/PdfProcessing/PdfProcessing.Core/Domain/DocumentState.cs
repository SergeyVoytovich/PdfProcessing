namespace PdfProcessing.Domain;

/// <summary>
/// State of Pfd document
/// </summary>
public enum DocumentState
{
    Unknown = 0,

    Received,
    Processing,
    Processed,

    Failed = int.MaxValue
}
