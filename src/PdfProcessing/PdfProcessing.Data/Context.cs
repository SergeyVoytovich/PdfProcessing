using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PdfProcessing.Data.Entities;

namespace PdfProcessing.Data;

internal class Context : DbContext, IContext
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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
		var entities = ChangeTracker.Entries<EntityBase>();
		await ProvideAuditAsync(entities, cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

	protected virtual async Task ProvideAuditAsync(IEnumerable<EntityEntry<EntityBase>> entires, CancellationToken cancellationToken = default)
	{
		foreach (var entiry in entires)
		{
			if(cancellationToken.IsCancellationRequested)
			{
				return;
			}

			await ProvideAuditAsync(entiry);
        }
    }

    protected virtual async Task ProvideAuditAsync(EntityEntry<EntityBase> entry)
    {
        var now = DateTime.UtcNow;
		var user = "system"; // temp solution

		if(entry.State == EntityState.Added)
		{
			entry.Entity.CreateBy = user;
			entry.Entity.CreatedAt = now;
        }

        if (entry.State == EntityState.Modified)
		{
			entry.Entity.UpdatedBy = user;
			entry.Entity.UpdatedAt = now;
        }

		//todo add for soft delete
  //      if(entry.State == EntityState.Deleted)
		//{
		//	entry.State = EntityState.Modified;
		//	entry.Entity.DeletedBy = user;
		//	entry.Entity.DeletedAt = now;
  //      }
    }

	public Task SaveAsync(CancellationToken cancellationToken) => SaveChangesAsync(cancellationToken);
}
