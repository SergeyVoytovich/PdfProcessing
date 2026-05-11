using System.Text.Json.Serialization;

namespace PdfProcessing.Application.Dtos;

public record PageContentDto
{
    [JsonPropertyName("page")]
    public int PageNumber { get; set; }

    [JsonPropertyName("content")]
    public string TextContent { get; set; } = string.Empty;
}