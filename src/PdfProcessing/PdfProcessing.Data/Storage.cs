using Microsoft.Extensions.DependencyInjection;
using PdfProcessing.Application.Data;
using PdfProcessing.Application.Data.Repositories;

namespace PdfProcessing.Data;

internal class Storage(IServiceProvider services) : IStorage
{
    private readonly Lazy<IDocumentsRepository> _documents = new(services.GetRequiredService<IDocumentsRepository>);
    public IDocumentsRepository Documents => _documents.Value;

    private readonly Lazy<IDocumentContentsRepository> _documentContents = new(services.GetRequiredService<IDocumentContentsRepository>);
    public IDocumentContentsRepository DocumentContents => _documentContents.Value;

    private readonly Lazy<IFileStorage> _files = new(services.GetRequiredService<IFileStorage>);
    public IFileStorage Files => _files.Value;
}
