using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PdfProcessing.Data.Entities;
using PdfProcessing.Data.Mapping;
using PdfProcessing.Data.Repositories;
using PdfProcessing.Domain;

namespace PdfProcessing.Data.UnitTests.Repositories;

public class CrudRepositoryBaseTests
{
    private sealed class TestCrudRepository(Context context, IMapper mapper)
        : CrudRepositoryBase<Document, DocumentEntity>(context, mapper, c => c.Documents)
    {
        public Task Add(Document document, CancellationToken cancellationToken = default)
            => AddAsync(document, cancellationToken);

        public Task Update(Document document, CancellationToken cancellationToken = default)
            => UpdateAsync(document, cancellationToken);

        public Task Delete(Guid id, CancellationToken cancellationToken = default)
            => DeleteAsync(id, cancellationToken);
    }

    private static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(
            cfg =>
            {
                cfg.AddProfile<GeneralProfile>();
                cfg.AddProfile<DocumentsProfile>();
            },
            NullLoggerFactory.Instance);

        return configuration.CreateMapper();
    }

    private static Context CreateContext()
    {
        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new Context(options);
    }

    private static TestCrudRepository CreateRepository(Context context)
        => new(context, CreateMapper());

    private static Document CreateDocument(DocumentState state = DocumentState.Received)
        => new()
        {
            Id = Guid.NewGuid(),
            DisplayName = "document.pdf",
            FilePath = "documents/document.pdf",
            State = state
        };

    private static DocumentEntity CreateEntity(DocumentState state = DocumentState.Received)
        => new()
        {
            Id = Guid.NewGuid(),
            DisplayName = "entity.pdf",
            FilePath = "documents/entity.pdf",
            State = (int)state
        };

    [Fact]
    public async Task AddAsync_AddsMappedEntity()
    {
        await using var context = CreateContext();
        var repository = CreateRepository(context);
        var document = CreateDocument(DocumentState.Processing);

        await repository.Add(document, CancellationToken.None);

        var entity = await context.Documents.SingleAsync();
        Assert.Equal(document.Id, entity.Id);
        Assert.Equal(document.DisplayName, entity.DisplayName);
        Assert.Equal(document.FilePath, entity.FilePath);
        Assert.Equal((int)document.State, entity.State);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityExists_UpdatesMappedEntity()
    {
        await using var context = CreateContext();
        var entity = CreateEntity(DocumentState.Received);
        await context.Documents.AddAsync(entity);
        await context.SaveAsync(CancellationToken.None);
        var repository = CreateRepository(context);
        var document = new Document
        {
            Id = entity.Id,
            DisplayName = "updated.pdf",
            FilePath = "documents/updated.pdf",
            State = DocumentState.Processed
        };

        await repository.Update(document, CancellationToken.None);

        var updated = await context.Documents.SingleAsync(i => i.Id == entity.Id);
        Assert.Equal(document.DisplayName, updated.DisplayName);
        Assert.Equal(document.FilePath, updated.FilePath);
        Assert.Equal((int)document.State, updated.State);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityDoesNotExist_ThrowsEntityNotFoundException()
    {
        await using var context = CreateContext();
        var repository = CreateRepository(context);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => repository.Update(CreateDocument(), CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityExists_MarksEntityAsDeleted()
    {
        await using var context = CreateContext();
        var entity = CreateEntity();
        await context.Documents.AddAsync(entity);
        await context.SaveAsync(CancellationToken.None);
        var repository = CreateRepository(context);

        await repository.Delete(entity.Id, CancellationToken.None);

        var deleted = await context.Documents.SingleAsync(i => i.Id == entity.Id);
        Assert.NotNull(deleted.DeletedAt);
        Assert.Equal("system", deleted.DeletedBy);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityDoesNotExist_ThrowsEntityNotFoundException()
    {
        await using var context = CreateContext();
        var repository = CreateRepository(context);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => repository.Delete(Guid.NewGuid(), CancellationToken.None));
    }
}
