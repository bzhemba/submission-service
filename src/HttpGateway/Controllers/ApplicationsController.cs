using ApplicationService;
using HttpGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace HttpGateway.Controllers;
[ApiController]
[Route("[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly ApplicationGrpcService _service;

    public ApplicationsController(ApplicationGrpcService applicationService)
    {
        _service = applicationService;
    }

    [HttpPost]
    [Route("/application/create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateApplication(
        [FromBody] CreateApplicationRequest createApplication,
        CancellationToken cancellationToken)
    {
         CreateApplicationResponse response = await _service.CreateApplicationAsync(createApplication, cancellationToken);
         return Ok(response);
    }

    [HttpPut]
    [Route("/application/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CancelApplication(
        [FromBody] long applicationId,
        CancellationToken cancellationToken)
    {
        await _service.CancelApplicationAsync(applicationId, cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("/application/edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> EditApplication(
        EditApplicationRequest editApplication,
        CancellationToken cancellationToken)
    {
         await _service.EditApplicationAsync(editApplication, cancellationToken);

         return Ok();
    }

    [HttpPut]
    [Route("/application/send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SendApplication(
        long applicationId,
        CancellationToken cancellationToken)
    {
         await _service.SendApplicationAsync(applicationId, cancellationToken);

         return Ok();
    }
}