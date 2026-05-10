using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PdfProcessing.Data.Entities;
using PdfProcessing.Domain;

namespace PdfProcessing.Data.Repositories;

internal abstract class CrudRepositoryBase<TDomain, TEntity> : RepositoryBase<TDomain, TEntity>
    where TDomain : DomainBase
    where TEntity : EntityBase
{
    protected DbSet<TEntity> DbSet { get;  }

    protected CrudRepositoryBase(IContext context, IMapper mapper, Func<IContext, DbSet<TEntity>> dbSet)
        :base(context, mapper)
    {
        DbSet = dbSet(context);
    }

    protected virtual async Task AddAsync(TDomain domain)
    {
        var entity = ConvertToEntity(domain);
        await DbSet.AddAsync(entity);
        await Context.SaveAsync();
    }

    protected virtual async Task UpdateAsync(TDomain domain)
    {
        var entity = await DbSet.FindAsync(domain.Id) ?? throw new EntityNotFoundException(domain.Id);
        Mapper.Map(domain, entity);
        await Context.SaveAsync();
    }

    protected virtual async Task DeleteAsync(Guid id)
    {
        var entity = await DbSet.FindAsync(id) ?? throw new EntityNotFoundException(id);
        DbSet.Remove(entity);
        await Context.SaveAsync();
    }
}