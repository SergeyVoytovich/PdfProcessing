using PdfProcessing.Application.Dtos;

namespace PdfProcessing.Application.Services;

public interface IDocumentsService
{
    public Task<IList<DocumentDto>> GetAsync(CancellationToken cancellationToken = default);
    public Task<DocumentContentDto?> GetContentAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<DocumentDto> AddAsync(string fileName, Stream stream, CancellationToken cancellationToken = default);
}
