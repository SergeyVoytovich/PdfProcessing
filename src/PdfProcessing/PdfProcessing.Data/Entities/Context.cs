using Microsoft.EntityFrameworkCore;

namespace PdfProcessing.Data.Entities;

internal class Context : DbContext
{
	#region DbSets

	public DbSet<DocumentEntity> Documents { get; set; }
	public DbSet<DocumentContentEntity> DocumentContents { get; set; }

	#endregion


	public Context(DbContextOptions<Context> options)
		: base(options)
	{ }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

		builder.Entity<DocumentContentEntity>()
			.HasOne(i => i.Document)
			.WithMany(i => i.Contents)
			.HasForeignKey(i => i.DocumentId)
			.OnDelete(DeleteBehavior.Cascade)
			;
    }
}
