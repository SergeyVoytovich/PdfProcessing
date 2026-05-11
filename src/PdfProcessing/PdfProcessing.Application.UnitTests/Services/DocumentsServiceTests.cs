using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PdfProcessing.Application.Data;
using PdfProcessing.Application.Data.Repositories;
using PdfProcessing.Application.Mapping;
using PdfProcessing.Application.Services;
using PdfProcessing.Data;
using PdfProcessing.Domain;
using PdfProcessing.Messaging;
using PdfProcessing.Messaging.Contracts;
using PdfProcessing.Utilities;

namespace PdfProcessing.Application.UnitTests.Services;

public class DocumentsServiceTests
{
    private sealed class TestDocumentsService(
        IStorage storage,
        IMapper mapper,
        IMessageBus messageBus,
        IContentExtractor extractor,
        ILogger<DocumentsService> logger)
        : DocumentsService(storage, mapper, messageBus, extractor, logger)
    {
        public Task AddDocumentContents(Document document, CancellationToken cancellationToken = default)
            => AddDocumentContentsAsync(document, cancellationToken);
    }

    private sealed record Dependencies(
        IStorage Storage,
        IFileStorage Files,
        IDocumentsRepository Documents,
        IDocumentContentsRepository DocumentContents,
        IMessageBus MessageBus,
        IContentExtractor Extractor);

    private static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(
            cfg => cfg.AddProfile<DtosProfile>(),
            NullLoggerFactory.Instance);

        return configuration.CreateMapper();
    }

    private static Dependencies CreateDependencies()
    {
        var storage = Substitute.For<IStorage>();
        var files = Substitute.For<IFileStorage>();
        var documents = Substitute.For<IDocumentsRepository>();
        var documentContents = Substitute.For<IDocumentContentsRepository>();
        var messageBus = Substitute.For<IMessageBus>();
        var extractor = Substitute.For<IContentExtractor>();

        storage.Files.Returns(files);
        storage.Documents.Returns(documents);
        storage.DocumentContents.Returns(documentContents);

        return new Dependencies(storage, files, documents, documentContents, messageBus, extractor);
    }

    private static DocumentsService CreateService(Dependencies dependencies)
        => new(
            dependencies.Storage,
            CreateMapper(),
            dependencies.MessageBus,
            dependencies.Extractor,
            NullLogger<DocumentsService>.Instance);

    private static TestDocumentsService CreateTestService(Dependencies dependencies)
        => new(
            dependencies.Storage,
            CreateMapper(),
            dependencies.MessageBus,
            dependencies.Extractor,
            NullLogger<DocumentsService>.Instance);

    [Fact]
    public async Task AddAsync_WhenParametersValid_AddsFileDocumentAndPublishesMessage()
    {
        var dependencies = CreateDependencies();
        var service = CreateService(dependencies);
        await using var stream = new MemoryStream([1, 2, 3]);
        dependencies.Files.AddAsync("document.pdf", stream, CancellationToken.None)
            .Returns("stored/document.pdf");

        var result = await service.AddAsync("document.pdf", stream, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("document.pdf", result.Name);
        Assert.Equal(DocumentState.Received.ToString(), result.State);
        await dependencies.Files.Received(1).AddAsync("document.pdf", stream, CancellationToken.None);
        await dependencies.Documents.Received(1).AddAsync(
            Arg.Is<Document>(document =>
                document.Id == result.Id &&
                document.DisplayName == "document.pdf" &&
                document.FilePath == "stored/document.pdf" &&
                document.State == DocumentState.Received),
            CancellationToken.None);
        await dependencies.MessageBus.Received(1).PublishAsync(
            Arg.Is<DocumentUploadedMessage>(message => message.DocumentId == result.Id),
            CancellationToken.None);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AddAsync_WhenFileNameMissing_ThrowsArgumentNullException(string? fileName)
    {
        var dependencies = CreateDependencies();
        var service = CreateService(dependencies);
        await using var stream = new MemoryStream();

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddAsync(fileName!, stream, CancellationToken.None));

        await dependencies.Files.DidNotReceiveWithAnyArgs().AddAsync(default!, default!, default);
        await dependencies.Documents.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
        await dependencies.MessageBus.DidNotReceiveWithAnyArgs().PublishAsync<DocumentUploadedMessage>(default!, default);
    }

    [Fact]
    public async Task AddAsync_WhenStreamIsNull_ThrowsArgumentNullException()
    {
        var dependencies = CreateDependencies();
        var service = CreateService(dependencies);

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddAsync("document.pdf", null!, CancellationToken.None));

        await dependencies.Files.DidNotReceiveWithAnyArgs().AddAsync(default!, default!, default);
        await dependencies.Documents.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
        await dependencies.MessageBus.DidNotReceiveWithAnyArgs().PublishAsync<DocumentUploadedMessage>(default!, default);
    }

    [Fact]
    public async Task AddAsync_WhenFileIsNotPdf_ThrowsInvalidOperationException()
    {
        var dependencies = CreateDependencies();
        var service = CreateService(dependencies);
        await using var stream = new MemoryStream();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.AddAsync("document.txt", stream, CancellationToken.None));

        Assert.Equal("Only PDF files are allowed.", exception.Message);
        await dependencies.Files.DidNotReceiveWithAnyArgs().AddAsync(default!, default!, default);
        await dependencies.Documents.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
        await dependencies.MessageBus.DidNotReceiveWithAnyArgs().PublishAsync<DocumentUploadedMessage>(default!, default);
    }

    [Fact]
    public async Task ProcessAsync_WhenDocumentDoesNotExist_DoesNothing()
    {
        var dependencies = CreateDependencies();
        var service = CreateService(dependencies);
        var documentId = Guid.NewGuid();
        dependencies.Documents.GetByIdAsync(documentId, CancellationToken.None)
            .Returns((Document?)null);

        await service.ProcessAsync(documentId, CancellationToken.None);

        await dependencies.Documents.Received(1).GetByIdAsync(documentId, CancellationToken.None);
        await dependencies.Documents.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        await dependencies.Extractor.DidNotReceiveWithAnyArgs().ExtractAsync(default!, default);
        await dependencies.DocumentContents.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
    }

    [Fact]
    public async Task ProcessAsync_WhenProcessingSucceeds_UpdatesStateAndAddsContents()
    {
        var dependencies = CreateDependencies();
        var service = CreateService(dependencies);
        var document = new Document
        {
            Id = Guid.NewGuid(),
            DisplayName = "document.pdf",
            FilePath = "stored/document.pdf",
            State = DocumentState.Received
        };
        var states = new List<DocumentState>();
        dependencies.Documents.GetByIdAsync(document.Id, CancellationToken.None)
            .Returns(document);
        dependencies.Documents
            .When(repository => repository.UpdateAsync(Arg.Any<Document>(), CancellationToken.None))
            .Do(call => states.Add(call.Arg<Document>().State));
        dependencies.Extractor.ExtractAsync(document.FilePath, Arg.Any<CancellationToken>())
            .Returns([new DocumentContent { PageNumber = 1, Content = "Page 1" }]);

        await service.ProcessAsync(document.Id, CancellationToken.None);

        Assert.Equal([DocumentState.Processing, DocumentState.Processed], states);
        await dependencies.DocumentContents.Received(1).AddAsync(
            Arg.Is<DocumentContent>(content =>
                content.DocumentId == document.Id &&
                content.PageNumber == 1 &&
                content.Content == "Page 1"),
            CancellationToken.None);
    }

    [Fact]
    public async Task ProcessAsync_WhenAddingContentsFails_MarksDocumentAsErrorAndRethrows()
    {
        var dependencies = CreateDependencies();
        var service = CreateService(dependencies);
        var document = new Document
        {
            Id = Guid.NewGuid(),
            DisplayName = "document.pdf",
            FilePath = "stored/document.pdf",
            State = DocumentState.Received
        };
        var states = new List<DocumentState>();
        dependencies.Documents.GetByIdAsync(document.Id, CancellationToken.None)
            .Returns(document);
        dependencies.Documents
            .When(repository => repository.UpdateAsync(Arg.Any<Document>(), CancellationToken.None))
            .Do(call => states.Add(call.Arg<Document>().State));
        dependencies.Extractor.ExtractAsync(document.FilePath, Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("extract failed"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ProcessAsync(document.Id, CancellationToken.None));

        Assert.Equal("extract failed", exception.Message);
        Assert.Equal([DocumentState.Processing, DocumentState.Error], states);
    }

    [Fact]
    public async Task AddDocumentContentsAsync_AddsExtractedContentsWithDocumentId()
    {
        var dependencies = CreateDependencies();
        var service = CreateTestService(dependencies);
        var document = new Document
        {
            Id = Guid.NewGuid(),
            FilePath = "stored/document.pdf"
        };
        dependencies.Extractor.ExtractAsync(document.FilePath, Arg.Any<CancellationToken>())
            .Returns([
                new DocumentContent { PageNumber = 1, Content = "Page 1" },
                new DocumentContent { PageNumber = 2, Content = "Page 2" }
            ]);

        await service.AddDocumentContents(document, CancellationToken.None);

        await dependencies.DocumentContents.Received(1).AddAsync(
            Arg.Is<DocumentContent>(content =>
                content.DocumentId == document.Id &&
                content.PageNumber == 1 &&
                content.Content == "Page 1"),
            CancellationToken.None);
        await dependencies.DocumentContents.Received(1).AddAsync(
            Arg.Is<DocumentContent>(content =>
                content.DocumentId == document.Id &&
                content.PageNumber == 2 &&
                content.Content == "Page 2"),
            CancellationToken.None);
    }

    [Fact]
    public async Task AddDocumentContentsAsync_WhenCancellationRequested_DoesNotAddContents()
    {
        var dependencies = CreateDependencies();
        var service = CreateTestService(dependencies);
        var document = new Document
        {
            Id = Guid.NewGuid(),
            FilePath = "stored/document.pdf"
        };
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        dependencies.Extractor.ExtractAsync(document.FilePath, Arg.Any<CancellationToken>())
            .Returns([new DocumentContent { PageNumber = 1, Content = "Page 1" }]);

        await service.AddDocumentContents(document, cancellation.Token);

        await dependencies.DocumentContents.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
    }

    [Fact]
    public async Task AddDocumentContentsAsync_WhenDocumentIsNull_ThrowsNullReferenceException()
    {
        var dependencies = CreateDependencies();
        var service = CreateTestService(dependencies);

        await Assert.ThrowsAsync<NullReferenceException>(
            () => service.AddDocumentContents(null!, CancellationToken.None));

        await dependencies.Extractor.DidNotReceiveWithAnyArgs().ExtractAsync(default!, default);
        await dependencies.DocumentContents.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
    }
}
