using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PdfProcessing.Application.Data.Repositories;
using PdfProcessing.Data.Entities;
using PdfProcessing.Data.Mapping;
using PdfProcessing.Domain;

namespace PdfProcessing.Data.Repositories;

internal class DocumentContentsRepository(IContext context, IMapper mapper)
    : CrudRepositoryBase<DocumentContent, DocumentContentEntity>(context, mapper, c => c.DocumentContents), IDocumentContentsRepository
{
    Task IDocumentContentsRepository.AddAsync(DocumentContent domain, CancellationToken cancellationToken) 
        => base.AddAsync(domain, cancellationToken);

    public Task<IList<DocumentContent>> GetByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default)
        => Context.DocumentContents
            .Where(i => i.DocumentId == documentId)
            .ToListAsync(cancellationToken)
            .MapAsync<DocumentContentEntity, DocumentContent>(Mapper);
}
