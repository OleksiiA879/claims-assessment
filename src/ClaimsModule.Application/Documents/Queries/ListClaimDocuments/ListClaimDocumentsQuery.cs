using ClaimsModule.Application.Claims.DTOs;
using MediatR;

namespace ClaimsModule.Application.Documents.Queries.ListClaimDocuments;

public sealed record ListClaimDocumentsQuery(Guid ClaimId) : IRequest<IReadOnlyList<DocumentDto>>;
