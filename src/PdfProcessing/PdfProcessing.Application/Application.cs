using Microsoft.Extensions.DependencyInjection;
using PdfProcessing.Application.Services;

namespace PdfProcessing.Application;

internal class Application(IServiceProvider services) : IApplication
{
    protected virtual IServiceProvider Services { get; } = services;

    private readonly Lazy<IDocumentsService> _documentsService = new(services.GetRequiredService<IDocumentsService>);

    public IDocumentsService Documents => _documentsService.Value;
}
