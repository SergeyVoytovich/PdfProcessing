using PdfProcessing.Api.Dtos;
using System.Text.Json.Serialization;

namespace PdfProcessing.Application.Dtos;

public record DocumentContentDto : DocumentDto
{
    [JsonPropertyName("contents")]
    public IList<PageContentDto> Pages { get; set; } = [];
}
