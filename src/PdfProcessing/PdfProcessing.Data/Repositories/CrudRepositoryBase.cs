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

    protected virtual async Task AddAsync(TDomain domain, CancellationToken cancellationToken)
    {
        var entity = ConvertToEntity(domain);
        await DbSet.AddAsync(entity, cancellationToken);
        await Context.SaveAsync(cancellationToken);
    }

    protected virtual async Task UpdateAsync(TDomain domain, CancellationToken cancellationToken)
    {
        var entity = await DbSet.FindAsync(domain.Id, cancellationToken) ?? throw new EntityNotFoundException(domain.Id);
        Mapper.Map(domain, entity);
        await Context.SaveAsync(cancellationToken);
    }

    protected virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await DbSet.FindAsync(id, cancellationToken) ?? throw new EntityNotFoundException(id);
        DbSet.Remove(entity);
        await Context.SaveAsync(cancellationToken);
    }
}