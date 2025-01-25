using Microsoft.AspNetCore.Mvc;
using SubmissionService.Application.Contracts;
using SubmissionService.Application.Contracts.Applications.Operations;

namespace SubmissionService.Presentation.Http.Controllers;
[ApiController]
[Route("[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _service;

    public ApplicationsController(IApplicationService applicationService)
    {
        _service = applicationService;
    }

    [HttpPost]
    [Route("/application/create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateApplication(
        [FromBody] CreateApplication.Request createApplication,
        CancellationToken cancellationToken)
    {
        CreateApplication.Result result = await _service.CreateAsync(createApplication, cancellationToken);
        return result switch
        {
            CreateApplication.Result.Success successCreateApplication => Ok(successCreateApplication.ApplicationId),
            Application.Contracts.Applications.Operations.CreateApplication.Result.DraftAlreadyExists => BadRequest(
                "Unable to create application. Draft already exists."),
            Application.Contracts.Applications.Operations.CreateApplication.Result.MissingRequiredFields => BadRequest(
                "Missing required fields"),
            _ => BadRequest("Unable to create application"),
        };
    }

    [HttpPost]
    [Route("/application/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CancelApplication(
        [FromBody] CancelApplication.Request cancelApplication,
        CancellationToken cancellationToken)
    {
        CancelApplication.Result result = await _service.CancelAsync(cancelApplication, cancellationToken);
        return result switch
        {
            CancelApplication.Result.Success successCreateApplication => Ok(),
            Application.Contracts.Applications.Operations.CancelApplication.Result.ApplicationNotFound => NotFound(
                $"Application {cancelApplication.ApplicationId} not found."),
            Application.Contracts.Applications.Operations.CancelApplication.Result.InvalidState => BadRequest(
                $"You can cancel the application only in state 'Pending approval'."),
            _ => BadRequest("Unable to cancel the application"),
        };
    }

    [HttpPost]
    [Route("/application/edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> EditApplication(
        EditApplication.Request editApplication,
        CancellationToken cancellationToken)
    {
        EditApplication.Result result = await _service.EditAsync(editApplication, cancellationToken);

        return result switch
        {
            Application.Contracts.Applications.Operations.EditApplication.Result.Success => Ok(),
            Application.Contracts.Applications.Operations.EditApplication.Result.ApplicationNotFound => NotFound(
                $"Application {editApplication.ApplicationId} not found."),
            Application.Contracts.Applications.Operations.EditApplication.Result.InvalidState => BadRequest(
                $"You can cancel the application only in state 'Draft'."),
            Application.Contracts.Applications.Operations.EditApplication.Result.MissingRequiredFields => BadRequest(
                $"Missing required fields."),
            _ => BadRequest("Unable to edit the application"),
        };
    }

    [HttpPost]
    [Route("/application/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ApproveApplication(
        ApproveApplication.Request approveApplication,
        CancellationToken cancellationToken)
    {
        ApproveApplication.Result result = await _service.ApproveAsync(approveApplication, cancellationToken);

        return result switch
        {
            Application.Contracts.Applications.Operations.ApproveApplication.Result.Success => Ok(),
            Application.Contracts.Applications.Operations.ApproveApplication.Result.ApplicationNotFound => NotFound(
                $"Application {approveApplication.ApplicationId} not found."),
            Application.Contracts.Applications.Operations.ApproveApplication.Result.InvalidState => BadRequest(
                $"You can cancel the application only in state 'Draft'."),
            _ => BadRequest("Unable to edit the application"),
        };
    }

    [HttpPost]
    [Route("/application/send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SendApplication(
        SendApplication.Request sendApplication,
        CancellationToken cancellationToken)
    {
        SendApplication.Result result = await _service.SendAsync(sendApplication, cancellationToken);

        return result switch
        {
            Application.Contracts.Applications.Operations.SendApplication.Result.Success => Ok(),
            Application.Contracts.Applications.Operations.SendApplication.Result.ApplicationNotFound => NotFound(
                $"Application {sendApplication.ApplicationId} not found."),
            _ => BadRequest("Unable to edit the application"),
        };
    }
}