using AutoMapper;

namespace PdfProcessing.Data.Mapping;

internal static class MapperExtensions
{
    public static async Task<TRes> MapAsync<TSrc, TRes>(this Task<TSrc?> action, IMapper mapper)
    {
        var src = await action;
        return mapper.Map<TRes>(src);
    }

    public static async Task<IList<TRes>> MapAsync<TSrc, TRes>(this Task<List<TSrc>> action, IMapper mapper)
    {
        var src = await action;
        return mapper.Map<IList<TRes>>(src);
    }
}
