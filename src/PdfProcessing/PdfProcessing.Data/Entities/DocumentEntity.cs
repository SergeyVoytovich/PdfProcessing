using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PdfProcessing.Data.Entities;

[Table("documents")]
internal record DocumentEntity : EntityBase
{
    [Required]
    [MaxLength(255)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    public int State { get; set; } 


    public virtual ICollection<DocumentContentEntity> Contents { get; set; } = new List<DocumentContentEntity>();
}
