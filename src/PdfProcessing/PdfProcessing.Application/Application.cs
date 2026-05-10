using PdfProcessing.Application.Services;

namespace PdfProcessing.Application;

internal class Application(IServiceProvider services) : IApplication
{
    protected virtual IServiceProvider Services { get; } = services;

    private readonly Lazy<IDocumentsService> _documentsService = new(() => throw new NotImplementedException());

    public IDocumentsService DocumentsService => _documentsService.Value;
}
