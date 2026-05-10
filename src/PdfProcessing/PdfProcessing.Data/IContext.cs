using Microsoft.EntityFrameworkCore;
using PdfProcessing.Data.Entities;

namespace PdfProcessing.Data;

internal interface IContext
{
    DbSet<DocumentEntity> Documents { get; }
    DbSet<DocumentContentEntity> DocumentContents { get; }

    Task SaveAsync(CancellationToken cancellationToken);
}
