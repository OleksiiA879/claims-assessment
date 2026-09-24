using ClaimsModule.Application.Claims.DTOs;
using MediatR;

namespace ClaimsModule.Application.Documents.Commands.UploadClaimDocument;

public sealed record UploadClaimDocumentCommand(
    Guid ClaimId,
    string DocumentType,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    Stream Content) : IRequest<DocumentDto>;
