using PdfProcessing.Application.Dtos;

namespace PdfProcessing.Application.Services;

public interface IDocumentsService
{
    Task<IList<DocumentDto>> GetAsync(CancellationToken cancellationToken = default);
    Task<DocumentContentDto?> GetContentAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DocumentDto> AddAsync(string fileName, Stream stream, CancellationToken cancellationToken = default);
    Task ProcessAsync(Guid documentId, CancellationToken cancellationToken = default);
}
