using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PdfProcessing.Application.Data;
using PdfProcessing.Application.Data.Repositories;
using PdfProcessing.Data.Mapping;
using PdfProcessing.Data.Repositories;

namespace PdfProcessing.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPdfProcessingData(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        => services.AddDbContext<Context>(options)
                .AddScoped<IContext>(p => p.GetRequiredService<Context>())
                .AddScoped<IStorage, Storage>()
                .AddScoped<IDocumentsRepository, DocumentsRepository>()
                .AddScoped<IDocumentContentsRepository, DocumentContentsRepository>()
                .AddAutoMapper(cnf =>
                {
                    cnf.AddProfile<GeneralProfile>();
                    cnf.AddProfile<DocumentsProfile>();
                })
        ;

    public static IServiceCollection AddPdfProcessingDataToNpsql(this IServiceCollection services, string connectionString)
        => services.AddPdfProcessingData(options => options.UseNpgsql(connectionString));
}
