using PdfProcessing.Data.Entities;

namespace PdfProcessing.Data.Repositories;

internal static class EntityBaseExtensions
{
    public static IQueryable<T> ById<T>(this IQueryable<T> query, Guid id) where T : EntityBase => query.Where(e => e.Id == id);
}
