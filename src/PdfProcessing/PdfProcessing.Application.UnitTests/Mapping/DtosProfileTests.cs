using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using PdfProcessing.Application.Dtos;
using PdfProcessing.Application.Mapping;
using PdfProcessing.Domain;

namespace PdfProcessing.Application.UnitTests.Mapping;

public class DtosProfileTests
{
    private static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(
            cfg => cfg.AddProfile<DtosProfile>(),
            NullLoggerFactory.Instance);

        return configuration.CreateMapper();
    }

    [Fact]
    public void Configuration_IsValid()
    {
        CreateMapper().ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void MapDocumentToDocumentDto()
    {
        var document = new Document
        {
            Id = Guid.NewGuid(),
            DisplayName = "document.pdf",
            FilePath = "documents/document.pdf",
            State = DocumentState.Processing
        };

        var result = CreateMapper().Map<DocumentDto>(document);

        Assert.Equal(document.Id, result.Id);
        Assert.Equal(document.DisplayName, result.Name);
        Assert.Equal(document.State.ToString(), result.State);
    }

    [Fact]
    public void MapDocumentToDocumentContentDto()
    {
        var document = new Document
        {
            Id = Guid.NewGuid(),
            DisplayName = "document.pdf",
            FilePath = "documents/document.pdf",
            State = DocumentState.Processed
        };

        var result = CreateMapper().Map<DocumentContentDto>(document);

        Assert.Equal(document.Id, result.Id);
        Assert.Equal(document.DisplayName, result.Name);
        Assert.Equal(document.State.ToString(), result.State);
        Assert.NotNull(result.Pages);
        Assert.Empty(result.Pages);
    }

    [Fact]
    public void MapDocumentContentToPageContentDto()
    {
        var content = new DocumentContent
        {
            Id = Guid.NewGuid(),
            DocumentId = Guid.NewGuid(),
            PageNumber = 7,
            Content = "Page text"
        };

        var result = CreateMapper().Map<PageContentDto>(content);

        Assert.Equal(content.PageNumber, result.PageNumber);
        Assert.Equal(content.Content, result.TextContent);
    }
}
