using Microsoft.AspNetCore.Mvc;
using PdfProcessing.Api.Routing;
using PdfProcessing.Application;
using PdfProcessing.Application.Dtos;

namespace PdfProcessing.Api.Controllers;

[Route(Routes.Documents.Base)]
public class DocumentsController(IApplication application) : AppControllerBase(application)
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<DocumentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IList<DocumentDto>>> Get(CancellationToken cancellationToken)
    {
        var result = await Application.Documents.GetAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DocumentDto>> Post(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null)
        {
            return BadRequest("File is required.");
        }

        var result = await Application.Documents.AddAscyn(file.FileName, file.OpenReadStream(), cancellationToken);
        return Ok(result);
    }
}
