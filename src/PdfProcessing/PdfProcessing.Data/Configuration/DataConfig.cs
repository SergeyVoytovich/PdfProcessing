using Microsoft.EntityFrameworkCore;

namespace PdfProcessing.Data.Configuration;

public class DataConfig
{
    public Action<DbContextOptionsBuilder> ContextBuilder { get; set; } = null!;
    public string FileStorageRootPath { get; set; } = null!;
}
