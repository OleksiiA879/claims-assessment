using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ClaimsModule.Persistence.Services;

public class ClaimNumberGenerator(ClaimsDbContext context) : IClaimNumberGenerator
{
    public async Task<string> GenerateNextAsync(Guid organisationId, CancellationToken cancellationToken = default)
    {
        var year = DateTime.UtcNow.Year;
        var connection = context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.Transaction = context.Database.CurrentTransaction?.GetDbTransaction();
        command.CommandText = """
            SET NOCOUNT ON;
            MERGE ClaimNumberSequences WITH (HOLDLOCK) AS target
            USING (SELECT @OrganisationId AS OrganisationId, @Year AS [Year]) AS source
            ON target.OrganisationId = source.OrganisationId AND target.[Year] = source.[Year]
            WHEN MATCHED THEN
                UPDATE SET LastSequence = target.LastSequence + 1
            WHEN NOT MATCHED THEN
                INSERT (OrganisationId, [Year], LastSequence) VALUES (source.OrganisationId, source.[Year], 1)
            OUTPUT inserted.LastSequence;
            """;

        var organisationParameter = command.CreateParameter();
        organisationParameter.ParameterName = "@OrganisationId";
        organisationParameter.Value = organisationId;
        command.Parameters.Add(organisationParameter);

        var yearParameter = command.CreateParameter();
        yearParameter.ParameterName = "@Year";
        yearParameter.Value = year;
        command.Parameters.Add(yearParameter);

        var sequence = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
        return ClaimNumber.Create(year, sequence).Value;
    }
}
