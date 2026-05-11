using AutoMapper;
using PdfProcessing.Data.Entities;
using PdfProcessing.Domain;

namespace PdfProcessing.Data.Mapping;

internal class DocumentsProfile : Profile
{
    public DocumentsProfile()
    {
        CreateMap<Document, DocumentEntity>()
            .IncludeBase<DomainBase, EntityBase>()
            .ForMember(dst => dst.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dst => dst.FilePath, opt => opt.MapFrom(src => src.FilePath))
            .ForMember(dst => dst.State, opt => opt.MapFrom(src => src.State))
            .ReverseMap()
            ;

        CreateMap<DocumentContent, DocumentContentEntity>()
           .IncludeBase<DomainBase, EntityBase>()
           .ForMember(dst => dst.DocumentId, opt => opt.MapFrom(src => src.DocumentId))
           .ForMember(dst => dst.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
           .ForMember(dst => dst.Content, opt => opt.MapFrom(src => src.Content))
           .ReverseMap()
           ;
    }
}
