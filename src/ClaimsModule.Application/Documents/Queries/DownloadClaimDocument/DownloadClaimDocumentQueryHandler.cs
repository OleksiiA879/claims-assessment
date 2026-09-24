using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Documents.Queries.DownloadClaimDocument;

public sealed class DownloadClaimDocumentQueryHandler(IApplicationDbContext context, IStorageService storage)
    : IRequestHandler<DownloadClaimDocumentQuery, DownloadClaimDocumentResult>
{
    public async Task<DownloadClaimDocumentResult> Handle(DownloadClaimDocumentQuery request, CancellationToken cancellationToken)
    {
        var document = await context.ClaimDocuments.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId && d.ClaimId == request.ClaimId, cancellationToken)
            ?? throw new NotFoundException(nameof(ClaimDocument), request.DocumentId);

        var stream = await storage.OpenReadAsync(document.BlobPath, cancellationToken);
        return new DownloadClaimDocumentResult(stream, document.ContentType, document.DocumentName);
    }
}
