using Microsoft.AspNetCore.Mvc;
using PdfProcessing.Application;

namespace PdfProcessing.Api.Controllers;

[ApiController]
public abstract class AppControllerBase(IApplication application) : ControllerBase
{
    protected virtual IApplication Application { get; } = application;
}
