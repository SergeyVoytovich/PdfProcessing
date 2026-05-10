using System.ComponentModel.DataAnnotations;

namespace PdfProcessing.Data.Entities;

internal abstract record EntityBase
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    [MaxLength(255)]
    public string CreateBy { get; set; } = string.Empty;

    public DateTime? UpdatedAt { get; set; }

    [MaxLength(255)]
    public string? UpdatedBy { get; set; } = string.Empty;

    public DateTime? DeletedAt { get; set; }

    [MaxLength(255)]
    public string? DeletedBy { get; set; } = string.Empty;
}
