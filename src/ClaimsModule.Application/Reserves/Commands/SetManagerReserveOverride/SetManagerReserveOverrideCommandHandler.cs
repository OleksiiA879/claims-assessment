using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Reserves.Commands.SetManagerReserveOverride;

public sealed class SetManagerReserveOverrideCommandHandler(
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog) : IRequestHandler<SetManagerReserveOverrideCommand, Unit>
{
    public async Task<Unit> Handle(SetManagerReserveOverrideCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsInRole("manager"))
            throw new ValidationException("Role", "Only a manager can change the aggregate reserve override.");

        var claim = await context.Claims.FirstOrDefaultAsync(c => c.Id == request.ClaimId, cancellationToken)
            ?? throw new NotFoundException(nameof(Claim), request.ClaimId);

        claim.ManagerOverrideForReserves = request.Enabled;
        claim.UpdatedAt = DateTimeOffset.UtcNow;
        claim.UserModified = currentUser.UserId;
        await auditLog.LogAsync(claim.Id, "RESERVE_MANAGER_OVERRIDE_CHANGED",
            $"Manager reserve override {(request.Enabled ? "enabled" : "disabled")}: {request.Reason}",
            newValue: request.Enabled.ToString(), cancellationToken: cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
