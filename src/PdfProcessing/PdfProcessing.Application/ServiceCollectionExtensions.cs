using Microsoft.Extensions.DependencyInjection;
using PdfProcessing.Application.Services;

namespace PdfProcessing.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPdfProcessingApplication(this IServiceCollection services)
        => services
            .AddScoped<IDocumentsService, DocumentsService>()
        ;
}
