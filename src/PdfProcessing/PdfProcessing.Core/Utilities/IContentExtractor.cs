using PdfProcessing.Domain;

namespace PdfProcessing.Utilities;

public interface IContentExtractor
{
    public Task<IList<DocumentContent>> ExtractAsync(string filePath, CancellationToken cancellationToken = default);
}
