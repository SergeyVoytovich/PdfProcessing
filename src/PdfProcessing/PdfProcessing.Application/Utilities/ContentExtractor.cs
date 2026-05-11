using PdfProcessing.Domain;
using PdfProcessing.Utilities;
using UglyToad.PdfPig;

namespace PdfProcessing.Application.Utilities;

internal class ContentExtractor : IContentExtractor
{
    public Task<IList<DocumentContent>> ExtractAsync(string filePath, CancellationToken cancellationToken = default)
    {
        using var pdfDocument = PdfDocument.Open(filePath);
        IList<DocumentContent> result = Extract(pdfDocument).ToList();
        return Task.FromResult(result);
    }

    protected virtual IEnumerable<DocumentContent> Extract(PdfDocument document)
    {
        var pageNumber = 0;
        foreach (var page in document.GetPages())
        {
            yield return new DocumentContent
            {
                Id = Guid.NewGuid(),
                PageNumber = ++pageNumber,
                Content = page.Text
            }; 
        }
    }
}
