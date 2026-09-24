using ClaimsModule.Application.Documents.Commands.UploadClaimDocument;
using ClaimsModule.Application.Documents.Queries.DownloadClaimDocument;
using ClaimsModule.Application.Documents.Queries.ListClaimDocuments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsModule.API.Controllers;

[ApiController]
[Route("api/claims/{claimId:guid}/documents")]
[Authorize]
public sealed class DocumentsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(Guid claimId, CancellationToken ct) =>
        Ok(await mediator.Send(new ListClaimDocumentsQuery(claimId), ct));

    [HttpGet("{documentId:guid}/download")]
    public async Task<IActionResult> Download(Guid claimId, Guid documentId, CancellationToken ct)
    {
        var result = await mediator.Send(new DownloadClaimDocumentQuery(claimId, documentId), ct);
        return File(result.Content, result.ContentType, result.FileName, enableRangeProcessing: true);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<IActionResult> Upload(
        Guid claimId,
        [FromForm] string documentType,
        IFormFile file,
        CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        var result = await mediator.Send(new UploadClaimDocumentCommand(
            claimId,
            documentType,
            file.FileName,
            file.ContentType,
            file.Length,
            stream), ct);
        return Created(result.DownloadUrl ?? string.Empty, result);
    }
}
