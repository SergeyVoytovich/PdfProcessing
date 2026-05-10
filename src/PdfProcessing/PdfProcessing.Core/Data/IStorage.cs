using PdfProcessing.Application.Data.Repositories;

namespace PdfProcessing.Application.Data;

public interface IStorage
{
    public IDocumentsRepository Documents { get; }
    public IDocumentContentsRepository DocumentContents { get; }
}
