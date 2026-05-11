using AutoMapper;
using PdfProcessing.Application.Data;
using PdfProcessing.Application.Dtos;
using PdfProcessing.Data;
using PdfProcessing.Domain;
using PdfProcessing.Messaging;
using PdfProcessing.Messaging.Contracts;

namespace PdfProcessing.Application.Services;

internal class DocumentsService(IStorage storage, IMapper mapper, IMessageBus messageBus) : IDocumentsService
{
    protected virtual IStorage Storage { get; } = storage;
    protected virtual IMapper Mapper { get; } = mapper;
    protected virtual IMessageBus MessageBus { get; } = messageBus;

    public async Task<DocumentDto> AddAscyn(string fileName, Stream stream, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (!fileName.IsPdf())
        {
            throw new InvalidOperationException("Only PDF files are allowed.");
        }

        var path = await Storage.Files.AddAsync(fileName, stream, cancellationToken);
        var document = new Document
        {
            Id = Guid.NewGuid(),
            DisplayName = fileName,
            FilePath = path,
            State = DocumentState.Received
        };

        await Storage.Documents.AddAsync(document, cancellationToken);
        await MessageBus.PublishAsync(new DocumentUploadedMessage { DocumentId = document.Id }, cancellationToken);

        return Mapper.Map<DocumentDto>(document);
    }

    public async Task<DocumentContentDto?> GetContentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty.", nameof(id));
        }

        var document = await Storage.Documents.GetByIdAsync(id, cancellationToken);
        if (document is null)
        {
            return null;
        }

        var result = Mapper.Map<DocumentContentDto>(document);

        if (document.State == DocumentState.Processed)
        {
            result.Pages = await GetPagesAsync(document.Id, cancellationToken);
        }

        return result;
    }

    protected virtual async Task<IList<PageContentDto>> GetPagesAsync(Guid docuemtnId, CancellationToken cancellationToken = default)
    {
        var contents = await Storage.DocumentContents.GetByDocumentIdAsync(docuemtnId, cancellationToken);
        return Mapper.Map<IList<PageContentDto>>(contents);
    }


    public async Task<IList<DocumentDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var documents = await Storage.Documents.GetByStates([DocumentState.Received, DocumentState.Processing, DocumentState.Processed], cancellationToken);
        return Mapper.Map<IList<DocumentDto>>(documents);
    }


}
