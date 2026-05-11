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
    Task<Document?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default);
    Task<IList<Document>> GetByStates(IList<DocumentState> states, CancellationToken cancellationToken = default);

    Task AddAsync(Document document, CancellationToken cancellationToken = default);
    Task UpdateAsync(Document document, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
