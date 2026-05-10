using AutoMapper;
using PdfProcessing.Data.Entities;
using PdfProcessing.Domain;

namespace PdfProcessing.Data.Mapping;

internal class GeneralProfile : Profile
{
    public GeneralProfile()
    {
        CreateMap<DomainBase, EntityBase>()
            .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
            .ReverseMap()
            ;
    }
}
