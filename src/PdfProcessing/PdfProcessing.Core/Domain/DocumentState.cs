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

    Error = int.MaxValue - 1,

    Failed = int.MaxValue
}
