using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PdfProcessing.Data.Entities;

[Table("document_contents")]
internal record DocumentContentEntity : EntityBase
{
    [Required]
    public Guid DocumentId { get; set; }

    [ForeignKey(nameof(DocumentId))]
    public DocumentEntity Document { get; set; } = null!;

    [Required]
    public int PageNumber { get; set; }

    public string? Content { get; set; } = string.Empty;
}
