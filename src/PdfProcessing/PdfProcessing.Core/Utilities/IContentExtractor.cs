using PdfProcessing.Domain;

namespace PdfProcessing.Utilities;

public interface IContentExtractor
{
    public Task<IList<DocumentContent>> ExtractAscyn(string filePath, CancellationToken cancellationToken = default);
}
