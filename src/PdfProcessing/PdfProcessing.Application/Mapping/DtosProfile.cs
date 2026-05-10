using AutoMapper;
using PdfProcessing.Api.Dtos;
using PdfProcessing.Domain;

namespace PdfProcessing.Application.Mapping;

internal class DtosProfile : Profile
{
    public DtosProfile()
    {
        CreateMap<Document, DocumentDto>()
            .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dst => dst.State, opt => opt.MapFrom(src => src.State.ToString()))
            ;
    }
}
