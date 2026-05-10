using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PdfProcessing.Application.Data.Repositories;
using PdfProcessing.Data.Entities;
using PdfProcessing.Domain;

namespace PdfProcessing.Data.Repositories;

internal class DocumentsRepository(IContext context, IMapper mapper)
    : CrudRepositoryBase<Document, DocumentEntity>(context, mapper, c => c.Documents) , IDocumentsRepository
{
    public async Task<IList<Document>> GetAllAsync()
    {
        var entities = await Context.Documents.ToListAsync();
        return Mapper.Map<IList<Document>>(entities);
    }

    public async Task<Document> GetByIdAsync(Guid id)
    {
        var entity = await Context.Documents.ById(id).SingleOrDefaultAsync();
        return Mapper.Map<Document>(entity);
    }

    public async Task<IList<Document>> GetByState(DocumentState state)
    {
        var entities = await Context.Documents.Where(i => i.State == (int)state).ToListAsync();
        return Mapper.Map<IList<Document>>(entities);
    }

    Task IDocumentsRepository.AddAsync(Document document) => base.AddAsync(document);

    Task IDocumentsRepository.UpdateAsync(Document document) => base.UpdateAsync(document);

    Task IDocumentsRepository.DeleteAsync(Guid id) => base.DeleteAsync(id);
}
