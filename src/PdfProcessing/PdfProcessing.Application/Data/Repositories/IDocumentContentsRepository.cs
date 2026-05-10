using PdfProcessing.Domain;

namespace PdfProcessing.Application.Data.Repositories;

/// <summary>
/// Defines methods for accessing and managing the contents of documents in a repository.
/// </summary>
/// <remarks>Implementations of this interface provide asynchronous operations for retrieving, adding, and
/// deleting document contents by their identifiers. Methods may throw exceptions if provided arguments are invalid or
/// if the requested document does not exist.</remarks>
public interface IDocumentContentsRepository
{
    Task<DocumentContent> GetByDocumentIdAsync(Guid documentId);

    Task AddAsync(Document document);
    Task DeleteAsync(Guid Id);
}
