using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PdfProcessing.Application.Data.Repositories;
using PdfProcessing.Data.Entities;
using PdfProcessing.Data.Mapping;
using PdfProcessing.Data.Repositories;
using PdfProcessing.Domain;

namespace PdfProcessing.Data.UnitTests.Repositories;

public class DocumentContentsRepositoryTests
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

    private static IDocumentContentsRepository CreateRepository(Context context)
        => new DocumentContentsRepository(context, CreateMapper());

    private static DocumentContent CreateContent(Guid documentId)
        => new()
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            Content = "Document content"
        };

    private static DocumentContentEntity CreateEntity(Guid documentId, string content = "Document content")
        => new()
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            Content = content
        };

    [Fact]
    public async Task AddAsync_AddsDocumentContent()
    {
        await using var context = CreateContext();
        var repository = CreateRepository(context);
        var documentId = Guid.NewGuid();
        var content = CreateContent(documentId);

        await repository.AddAsync(content, CancellationToken.None);

        var entity = await context.DocumentContents.SingleAsync();
        Assert.Equal(content.Id, entity.Id);
        Assert.Equal(content.DocumentId, entity.DocumentId);
        Assert.Equal(content.Content, entity.Content);
    }

    [Fact]
    public async Task GetByDocumentIdAsync_ReturnsOnlyContentForRequestedDocument()
    {
        await using var context = CreateContext();
        var documentId = Guid.NewGuid();
        var otherDocumentId = Guid.NewGuid();
        var first = CreateEntity(documentId, "First content");
        var second = CreateEntity(documentId, "Second content");
        var other = CreateEntity(otherDocumentId, "Other content");
        await context.DocumentContents.AddRangeAsync(first, second, other);
        await context.SaveAsync(CancellationToken.None);
        var repository = CreateRepository(context);

        var result = await repository.GetByDocumentIdAsync(documentId, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, i => i.Id == first.Id && i.DocumentId == documentId && i.Content == first.Content);
        Assert.Contains(result, i => i.Id == second.Id && i.DocumentId == documentId && i.Content == second.Content);
        Assert.DoesNotContain(result, i => i.Id == other.Id);
    }

    [Fact]
    public async Task GetByDocumentIdAsync_WhenDocumentHasNoContent_ReturnsEmptyList()
    {
        await using var context = CreateContext();
        await context.DocumentContents.AddAsync(CreateEntity(Guid.NewGuid()));
        await context.SaveAsync(CancellationToken.None);
        var repository = CreateRepository(context);

        var result = await repository.GetByDocumentIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Empty(result);
    }
}
