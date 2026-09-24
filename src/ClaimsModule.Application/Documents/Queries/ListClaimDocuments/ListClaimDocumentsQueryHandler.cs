using ClaimsModule.Application.Claims.DTOs;
using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Documents.Queries.ListClaimDocuments;

public sealed class ListClaimDocumentsQueryHandler(IApplicationDbContext context, IStorageService storage)
    : IRequestHandler<ListClaimDocumentsQuery, IReadOnlyList<DocumentDto>>
{
    public async Task<IReadOnlyList<DocumentDto>> Handle(ListClaimDocumentsQuery request, CancellationToken cancellationToken)
    {
        if (!await context.Claims.AsNoTracking().AnyAsync(c => c.Id == request.ClaimId, cancellationToken))
            throw new NotFoundException(nameof(Claim), request.ClaimId);

        var documents = await context.ClaimDocuments.AsNoTracking()
            .Where(d => d.ClaimId == request.ClaimId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync(cancellationToken);

        var result = new List<DocumentDto>(documents.Count);
        foreach (var document in documents)
        {
            var url = await storage.GetDownloadUrlAsync(document.BlobPath, TimeSpan.FromHours(1), cancellationToken);
            if (url.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
                url = $"/api/claims/{request.ClaimId}/documents/{document.Id}/download";
            result.Add(new DocumentDto(document.Id, document.DocumentName, document.DocumentType,
                document.UploadedAt, document.FileSizeBytes, url));
        }

        return result;
    }
}
