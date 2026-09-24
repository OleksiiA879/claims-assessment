using FluentValidation;

namespace ClaimsModule.Application.Documents.Commands.UploadClaimDocument;

public sealed class UploadClaimDocumentCommandValidator : AbstractValidator<UploadClaimDocumentCommand>
{
    private static readonly string[] AllowedContentTypes =
        ["application/pdf", "image/jpeg", "image/png", "text/plain"];

    public UploadClaimDocumentCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
        RuleFor(x => x.DocumentType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType).Must(AllowedContentTypes.Contains)
            .WithMessage("Only PDF, JPEG, PNG, and text files are supported.");
        RuleFor(x => x.FileSizeBytes).GreaterThan(0).LessThanOrEqualTo(20 * 1024 * 1024)
            .WithMessage("Document size must be between 1 byte and 20 MB.");
    }
}
