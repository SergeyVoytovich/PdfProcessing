using Microsoft.AspNetCore.Mvc;
using PdfProcessing.Api.Dtos;
using PdfProcessing.Api.Routing;
using PdfProcessing.Application;

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
}
