using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PdfProcessing.Data.Entities;

namespace PdfProcessing.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPdfProcessingData(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        => services.AddDbContext<Context>(options);

    public static IServiceCollection AddPdfProcessingDataToNpsql(this IServiceCollection services, string connectionString)
        => services.AddPdfProcessingData(options => options.UseNpgsql(connectionString));
}
