using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PdfProcessing.Application.Data.Repositories;
using PdfProcessing.Data.Entities;
using PdfProcessing.Domain;

namespace PdfProcessing.Data.Repositories;

internal class DocumentsRepository : RepositoryBase<Document, DocumentEntity>, IDocumentsRepository
{
    public DocumentsRepository(IContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task AddAsync(Document document)
    {
        var entity = Mapper.Map<DocumentEntity>(document);

        await Context.Documents.AddAsync(entity);
        await Context.SaveAsync();
    }

    public Task DeleteAsync(Guid Id)
    {
        throw new NotImplementedException();
    }

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

    public async Task UpdateAsync(Document document)
    {
        var entity = await Context.Documents.ById(document.Id).SingleOrDefaultAsync() ?? throw new EntityNotFoundException(document.Id);
        entity = Mapper.Map(document, entity);

        await Context.SaveAsync();
    }
}
