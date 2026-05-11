using AutoMapper;
using PdfProcessing.Api.Dtos;
using PdfProcessing.Application.Data;
using PdfProcessing.Domain;

namespace PdfProcessing.Application.Services;

internal class DocumentsService(IStorage storage, IMapper mapper) : IDocumentsService
{
    protected virtual IStorage Storage { get; } = storage;
    protected virtual IMapper Mapper { get; } = mapper;

    public async Task<DocumentDto> AddAscyn(string fileName, Stream stream, CancellationToken cancellationToken = default)
    {
        var path = await Storage.Files.AddAsync(fileName, stream, cancellationToken);
        var document = new Document
        {
            Id = Guid.NewGuid(),
            DisplayName = fileName,
            FilePath = path,
            State = DocumentState.Received
        };

        await Storage.Documents.AddAsync(document, cancellationToken);

        return Mapper.Map<DocumentDto>(document);
    }

    public async Task<IList<DocumentDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var documents = await Storage.Documents.GetByStates([DocumentState.Received, DocumentState.Processing, DocumentState.Processed], cancellationToken);
        return Mapper.Map<IList<DocumentDto>>(documents);
    }
}
