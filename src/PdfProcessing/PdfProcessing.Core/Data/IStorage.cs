using PdfProcessing.Application.Data.Repositories;
using PdfProcessing.Data;

namespace PdfProcessing.Application.Data;

public interface IStorage
{
    public IFileStorage Files { get; }
    public IDocumentsRepository Documents { get; }
    public IDocumentContentsRepository DocumentContents { get; }
}
