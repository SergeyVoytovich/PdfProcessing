using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PdfProcessing.Application.Data;
using PdfProcessing.Application.Data.Repositories;
using PdfProcessing.Data.Configuration;
using PdfProcessing.Data.Mapping;
using PdfProcessing.Data.Repositories;

namespace PdfProcessing.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPdfProcessingData(this IServiceCollection services, DataConfig config)
        => services.AddDbContext<Context>(config.ContextBuilder)
                .AddScoped<IContext>(p => p.GetRequiredService<Context>())
                .AddScoped<IStorage, Storage>()
                .AddScoped<IDocumentsRepository, DocumentsRepository>()
                .AddScoped<IDocumentContentsRepository, DocumentContentsRepository>()
                .AddAutoMapper(cnf =>
                {
                    cnf.AddProfile<GeneralProfile>();
                    cnf.AddProfile<DocumentsProfile>();
                })
                .AddScoped<IFileStorage>(_ => new FileStorage(config.FileStorageRootPath))
        ;

    public static IServiceCollection AddPdfProcessingDataToNpsql(this IServiceCollection services, string connectionString)
        => services.AddPdfProcessingDataToNpsql(connectionString, Directory.GetCurrentDirectory());

    public static IServiceCollection AddPdfProcessingDataToNpsql(this IServiceCollection services, string connectionString, string fileStoragePath)
        => services.AddPdfProcessingData(new DataConfig 
        {
            ContextBuilder = options => options.UseNpgsql(connectionString),
            FileStorageRootPath = fileStoragePath
        });
}
