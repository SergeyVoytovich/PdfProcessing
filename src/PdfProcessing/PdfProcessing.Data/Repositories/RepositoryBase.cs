using AutoMapper;
using PdfProcessing.Data.Entities;
using PdfProcessing.Domain;

namespace PdfProcessing.Data.Repositories;

internal abstract class RepositoryBase<TDomain, TEntity>(IContext context, IMapper mapper)
    where TDomain : DomainBase
    where TEntity : EntityBase
{
    protected IContext Context { get; } = context ?? throw new ArgumentNullException(nameof(context));
    protected IMapper Mapper { get; } = mapper ?? throw new ArgumentNullException(nameof(mapper));

    protected virtual TEntity ConvertToEntity(TDomain domain) => Mapper.Map<TEntity>(domain);
    protected virtual TDomain ConvertToDomain(TEntity entity) => Mapper.Map<TDomain>(entity);
}
