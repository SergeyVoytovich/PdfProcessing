using PdfProcessing.Domain;

namespace PdfProcessing.Application.Data.Repositories;

public static class DocumentsRepositoryExtensions
{
    public static Task<IList<Document>> GetProcessedOnly(this IDocumentsRepository repository)
        => repository.GetByState(DocumentState.Processed);
}
