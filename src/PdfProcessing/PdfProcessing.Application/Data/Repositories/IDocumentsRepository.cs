using PdfProcessing.Domain;

namespace PdfProcessing.Application.Data.Repositories;

/// <summary>
/// Defines the contract for a repository that provides asynchronous operations for managing documents.
/// </summary>
/// <remarks>This interface abstracts the data access layer for documents, enabling retrieval, addition, update,
/// and deletion operations. Implementations are expected to handle persistence and concurrency concerns as appropriate
/// for the underlying data store.</remarks>
public interface IDocumentsRepository
{
    Task<Document> GetByIdAsync(Guid Id);
    Task<IList<Document>> GetByState(DocumentState state);
    Task<IList<Document>> GetByStates(IList<DocumentState> states);

    Task AddAsync(Document document);
    Task UpdateAsync(Document document);
    Task DeleteAsync(Guid id);
}
