using Library.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/info")]
public sealed class InfoController(IOptions<LibraryOptions> options, IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        options.Value.Name,
        options.Value.MaxLoansPerMember,
        Environment = environment.EnvironmentName
    });
}