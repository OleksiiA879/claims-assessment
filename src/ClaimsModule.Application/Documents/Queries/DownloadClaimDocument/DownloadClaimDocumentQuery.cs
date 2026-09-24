using MediatR;

namespace ClaimsModule.Application.Documents.Queries.DownloadClaimDocument;

public sealed record DownloadClaimDocumentResult(Stream Content, string ContentType, string FileName);
public sealed record DownloadClaimDocumentQuery(Guid ClaimId, Guid DocumentId) : IRequest<DownloadClaimDocumentResult>;
