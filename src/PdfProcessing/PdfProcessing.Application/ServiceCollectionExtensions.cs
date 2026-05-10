using Microsoft.Extensions.DependencyInjection;
using PdfProcessing.Application.Mapping;
using PdfProcessing.Application.Services;

namespace PdfProcessing.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPdfProcessingApplication(this IServiceCollection services)
        => services
            .AddScoped<IDocumentsService, DocumentsService>()
            .AddScoped<IApplication, Application>()
            .AddAutoMapper(cnf => cnf.AddProfile<DtosProfile>())
        ;
}
