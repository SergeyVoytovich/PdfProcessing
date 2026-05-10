using System.Text.Json.Serialization;

namespace PdfProcessing.Api.Dtos;

public abstract record DtoBase
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
}
