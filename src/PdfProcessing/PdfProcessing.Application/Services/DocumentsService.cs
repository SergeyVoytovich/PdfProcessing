using AutoMapper;
using PdfProcessing.Application.Data;
using PdfProcessing.Application.Dtos;
using PdfProcessing.Data;
using PdfProcessing.Domain;
using PdfProcessing.Messaging;
using PdfProcessing.Messaging.Contracts;
using PdfProcessing.Utilities;

namespace PdfProcessing.Application.Services;

internal class DocumentsService(IStorage storage, IMapper mapper, IMessageBus messageBus, IContentExtractor extractor) : IDocumentsService
{
    protected virtual IStorage Storage { get; } = storage;
    protected virtual IMapper Mapper { get; } = mapper;
    protected virtual IMessageBus MessageBus { get; } = messageBus;
    protected virtual IContentExtractor Extractor { get; } = extractor;

    public async Task<DocumentDto> AddAsync(string fileName, Stream stream, CancellationToken cancellationToken = default)
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

    public async Task ProcessAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var document = await Storage.Documents.GetByIdAsync(documentId, cancellationToken);
        if(document is null)
        {
            return;
        }

        document.State = DocumentState.Processing;
        await Storage.Documents.UpdateAsync(document, cancellationToken);

        try
        {
            await AddDocumentContentsAsync(document, cancellationToken);

            document.State = DocumentState.Processed;
            await Storage.Documents.UpdateAsync(document, cancellationToken);
        }
        catch (Exception)
        {
            document.State = DocumentState.Error;
            await Storage.Documents.UpdateAsync(document, cancellationToken);
            throw;
        }
    }

    protected virtual async Task AddDocumentContentsAsync(Document document, CancellationToken cancellationToken = default)
    {
        foreach (var content in await Extractor.ExtractAsync(document.FilePath))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            content.DocumentId = document.Id;
            await Storage.DocumentContents.AddAsync(content, cancellationToken);
        }
    }
}
