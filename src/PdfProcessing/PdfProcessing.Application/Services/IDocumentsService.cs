using PdfProcessing.Api.Dtos;

namespace PdfProcessing.Application.Services;

public interface IDocumentsService
{
    public Task<IList<DocumentDto>> GetAsync(CancellationToken cancellationToken = default);
}
