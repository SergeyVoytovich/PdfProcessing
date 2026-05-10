using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PdfProcessing.Data.Entities;
using PdfProcessing.Data.Mapping;
using PdfProcessing.Data.Repositories;
using PdfProcessing.Domain;

namespace PdfProcessing.Data.UnitTests.Repositories;

public class DocumentsRepositoryTests
{
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

    private static DocumentsRepository CreateRepository(Context context)
        => new(context, CreateMapper());

    private static DocumentEntity CreateEntity(DocumentState state = DocumentState.Received)
        => new()
        {
            Id = Guid.NewGuid(),
            DisplayName = "entity.pdf",
            FilePath = "documents/entity.pdf",
            State = (int)state
        };

    [Fact]
    public async Task GetAllAsync_ReturnsAllDocuments()
    {
        await using var context = CreateContext();
        var first = CreateEntity(DocumentState.Received);
        var second = CreateEntity(DocumentState.Processed);
        await context.Documents.AddRangeAsync(first, second);
        await context.SaveAsync();
        var repository = CreateRepository(context);

        var result = await repository.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, i => i.Id == first.Id);
        Assert.Contains(result, i => i.Id == second.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenDocumentExists_ReturnsDocument()
    {
        await using var context = CreateContext();
        var entity = CreateEntity(DocumentState.Processed);
        await context.Documents.AddAsync(entity);
        await context.SaveAsync();
        var repository = CreateRepository(context);

        var result = await repository.GetByIdAsync(entity.Id);

        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.DisplayName, result.DisplayName);
        Assert.Equal(entity.FilePath, result.FilePath);
        Assert.Equal((DocumentState)entity.State, result.State);
    }

    [Fact]
    public async Task GetByIdAsync_WhenDocumentDoesNotExist_ReturnsNull()
    {
        await using var context = CreateContext();
        var repository = CreateRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByState_ReturnsOnlyDocumentsWithRequestedState()
    {
        await using var context = CreateContext();
        var processed = CreateEntity(DocumentState.Processed);
        var failed = CreateEntity(DocumentState.Failed);
        await context.Documents.AddRangeAsync(processed, failed);
        await context.SaveAsync();
        var repository = CreateRepository(context);

        var result = await repository.GetByState(DocumentState.Processed);

        var document = Assert.Single(result);
        Assert.Equal(processed.Id, document.Id);
        Assert.Equal(DocumentState.Processed, document.State);
    }
}
