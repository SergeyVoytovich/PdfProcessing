using PdfProcessing.Domain;

namespace PdfProcessing.Application.Data.Repositories;

public interface IDocumentsRepository
{
    Task<Document> GetByIdAsync(Guid Id);
    Task<IList<Document>> GetAllAsync();

    Task AddAsync(Document document);
    Task UpdateAsync(Document document);
    Task DeleteAsync(Guid Id);
}
