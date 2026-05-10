using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using PdfProcessing.Data.Entities;
using PdfProcessing.Data.Mapping;
using PdfProcessing.Domain;

namespace PdfProcessing.Data.UnitTests.Mapping;

public class DocumentsProfileTests
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

    [Fact]
    public void MapDocumentToDocumentEntity()
    {
        var document = new Document
        {
            Id = Guid.NewGuid(),
            DisplayName = "display-name.pdf",
            FilePath = "documents/display-name.pdf",
            State = DocumentState.Processing
        };

        var result = CreateMapper().Map<DocumentEntity>(document);

        Assert.Equal(document.Id, result.Id);
        Assert.Equal(document.DisplayName, result.DisplayName);
        Assert.Equal(document.FilePath, result.FilePath);
        Assert.Equal((int)document.State, result.State);
    }

    [Fact]
    public void MapDocumentEntityToDocument()
    {
        var entity = new DocumentEntity
        {
            Id = Guid.NewGuid(),
            DisplayName = "display-name.pdf",
            FilePath = "documents/display-name.pdf",
            State = (int)DocumentState.Processed
        };

        var result = CreateMapper().Map<Document>(entity);

        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.DisplayName, result.DisplayName);
        Assert.Equal(entity.FilePath, result.FilePath);
        Assert.Equal((DocumentState)entity.State, result.State);
    }

    [Fact]
    public void MapDocumentContentToDocumentContentEntity()
    {
        var content = new DocumentContent
        {
            Id = Guid.NewGuid(),
            DocumentId = Guid.NewGuid(),
            Content = "Document content"
        };

        var result = CreateMapper().Map<DocumentContentEntity>(content);

        Assert.Equal(content.Id, result.Id);
        Assert.Equal(content.DocumentId, result.DocumentId);
        Assert.Equal(content.Content, result.Content);
    }

    [Fact]
    public void MapDocumentContentEntityToDocumentContent()
    {
        var entity = new DocumentContentEntity
        {
            Id = Guid.NewGuid(),
            DocumentId = Guid.NewGuid(),
            Content = "Document content"
        };

        var result = CreateMapper().Map<DocumentContent>(entity);

        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.DocumentId, result.DocumentId);
        Assert.Equal(entity.Content, result.Content);
    }

    
}
