using ClaimsModule.Application.Claims.DTOs;
using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Documents.Commands.UploadClaimDocument;

public sealed class UploadClaimDocumentCommandHandler(
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    IStorageService storage,
    ICurrentUserService currentUser,
    IAuditLogService auditLog) : IRequestHandler<UploadClaimDocumentCommand, DocumentDto>
{
    public async Task<DocumentDto> Handle(UploadClaimDocumentCommand request, CancellationToken cancellationToken)
    {
        if (!await context.Claims.AnyAsync(c => c.Id == request.ClaimId, cancellationToken))
            throw new NotFoundException(nameof(Claim), request.ClaimId);

        var documentId = Guid.NewGuid();
        var safeName = Path.GetFileName(request.FileName);
        var blobPath = $"{currentUser.OrganisationId:N}/{request.ClaimId:N}/{documentId:N}/{safeName}";
        await storage.UploadAsync(blobPath, request.Content, request.ContentType, cancellationToken);

        try
        {
            var document = new ClaimDocument
            {
                Id = documentId,
                OrganisationId = currentUser.OrganisationId,
                ClaimId = request.ClaimId,
                DocumentType = request.DocumentType.Trim(),
                DocumentName = safeName,
                BlobPath = blobPath,
                ContentType = request.ContentType,
                FileSizeBytes = request.FileSizeBytes,
                UploadedAt = DateTimeOffset.UtcNow,
                UploadedByUserId = currentUser.UserId,
                CreatedAt = DateTimeOffset.UtcNow,
                UserCreated = currentUser.UserId
            };

            context.ClaimDocuments.Add(document);
            await auditLog.LogAsync(request.ClaimId, "DOCUMENT_UPLOADED",
                $"Document {safeName} uploaded.",
                relatedEntityId: document.Id, relatedEntityType: nameof(ClaimDocument),
                cancellationToken: cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var downloadUrl = await storage.GetDownloadUrlAsync(blobPath, TimeSpan.FromHours(1), cancellationToken);
            if (downloadUrl.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
                downloadUrl = $"/api/claims/{request.ClaimId}/documents/{document.Id}/download";
            return new DocumentDto(document.Id, safeName, document.DocumentType, document.UploadedAt, document.FileSizeBytes, downloadUrl);
        }
        catch
        {
            await storage.DeleteAsync(blobPath, cancellationToken);
            throw;
        }
    }
}
