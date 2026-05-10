using AutoMapper;
using PdfProcessing.Api.Dtos;
using PdfProcessing.Application.Data;
using PdfProcessing.Domain;

namespace PdfProcessing.Application.Services;

internal class DocumentsService(IStorage storage, IMapper mapper) : IDocumentsService
{
    protected virtual IStorage Storage { get; } = storage;
    protected virtual IMapper Mapper { get; } = mapper;



    public async Task<IList<DocumentDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var documents = await Storage.Documents.GetByStates([DocumentState.Received, DocumentState.Processing, DocumentState.Processed], cancellationToken);
        return Mapper.Map<IList<DocumentDto>>(documents);
    }
}
