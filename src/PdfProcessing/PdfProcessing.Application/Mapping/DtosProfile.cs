using AutoMapper;
using PdfProcessing.Api.Dtos;
using PdfProcessing.Application.Dtos;
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

        CreateMap<Document, DocumentContentDto>()
            .IncludeBase<Document, DocumentDto>()
            ;

        CreateMap<DocumentContent, PageContentDto>()
            .ForMember(dst => dst.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
            .ForMember(dst => dst.TextContent, opt => opt.MapFrom(src => src.Content))
            ;
    }
}
