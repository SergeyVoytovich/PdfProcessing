using PdfProcessing.Application.Services;

namespace PdfProcessing.Application;

public interface IApplication
{
    public IDocumentsService DocumentsService { get; }
}
