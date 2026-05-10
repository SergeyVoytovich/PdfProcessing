using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PdfProcessing.Application.Data.Repositories;
using PdfProcessing.Data.Entities;
using PdfProcessing.Data.Mapping;
using PdfProcessing.Domain;

namespace PdfProcessing.Data.Repositories;

internal class DocumentsRepository(IContext context, IMapper mapper)
    : CrudRepositoryBase<Document, DocumentEntity>(context, mapper, c => c.Documents), IDocumentsRepository
{
    public Task<Document> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Context.Documents
                .ById(id)
                .SingleOrDefaultAsync(cancellationToken)
                .MapAsync<DocumentEntity, Document>(Mapper);

    public Task<IList<Document>> GetByState(DocumentState state, CancellationToken cancellationToken = default) 
        => Context.Documents
                .Where(i => i.State == (int)state)
                .ToListAsync(cancellationToken)
                .MapAsync<DocumentEntity, Document>(Mapper);

    Task IDocumentsRepository.AddAsync(Document document, CancellationToken cancellationToken)
        => base.AddAsync(document, cancellationToken);

    Task IDocumentsRepository.UpdateAsync(Document document, CancellationToken cancellationToken)
        => base.UpdateAsync(document, cancellationToken);

    Task IDocumentsRepository.DeleteAsync(Guid id, CancellationToken cancellationToken) 
        => base.DeleteAsync(id, cancellationToken);

    public Task<IList<Document>> GetByStates(IList<DocumentState> states, CancellationToken cancellationToken = default)
        => Context.Documents
                .AsNoTracking()
                .Where(i => states.Select(s => (int)s).Contains(i.State))
                .ToListAsync(cancellationToken)
                .MapAsync<DocumentEntity, Document>(Mapper);
}
