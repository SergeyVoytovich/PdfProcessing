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
    public Task<Document> GetByIdAsync(Guid id)
        => Context.Documents
                .ById(id)
                .SingleOrDefaultAsync()
                .MapAsync<DocumentEntity, Document>(Mapper);

    public Task<IList<Document>> GetByState(DocumentState state) 
        => Context.Documents
                .Where(i => i.State == (int)state)
                .ToListAsync()
                .MapAsync<DocumentEntity, Document>(Mapper);

    Task IDocumentsRepository.AddAsync(Document document) => base.AddAsync(document);

    Task IDocumentsRepository.UpdateAsync(Document document) => base.UpdateAsync(document);

    Task IDocumentsRepository.DeleteAsync(Guid id) => base.DeleteAsync(id);

    public Task<IList<Document>> GetByStates(IList<DocumentState> states)
        => Context.Documents
                .Where(i => states.Select(s => (int)s).Contains(i.State))
                .ToListAsync()
                .MapAsync<DocumentEntity, Document>(Mapper);
}
