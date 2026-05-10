using AutoMapper;

namespace PdfProcessing.Data.Repositories;

internal abstract class RepositoryBase<TDomain, TEntity>
{
    protected IContext Context { get; }
    protected IMapper Mapper { get; }


    protected RepositoryBase(IContext context, IMapper mapper)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    protected virtual TEntity ConvertToEntity(TDomain domain) => Mapper.Map<TEntity>(domain);
    protected virtual TDomain ConvertToDomain(TEntity entity) => Mapper.Map<TDomain>(entity);

}
