using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PdfProcessing.Data.Entities;

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
		await ProvideAuditAsync(entities);

        return await base.SaveChangesAsync(cancellationToken);
    }

	protected virtual async Task ProvideAuditAsync(IEnumerable<EntityEntry<EntityBase>> entires)
	{
		foreach (var entiry in entires)
		{
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

        if(entry.State == EntityState.Deleted)
		{
			entry.State = EntityState.Modified;
			entry.Entity.DeletedBy = user;
			entry.Entity.DeletedAt = now;
        }
    }

	public Task SaveAsync() => SaveChangesAsync();
}
