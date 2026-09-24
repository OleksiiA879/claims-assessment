using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClaimsModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompleteAssessmentSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReserveHistory",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Policies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Policies",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Policies",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Policies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Policies",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserCreated",
                table: "Policies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserModified",
                table: "Policies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ClaimValidationIssues",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ClaimStatusTransitions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "ClaimStatusTransitions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "ClaimStatusTransitions",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ClaimStatusTransitions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "ClaimStatusTransitions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ClaimStatusTransitions",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserCreated",
                table: "ClaimStatusTransitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserModified",
                table: "ClaimStatusTransitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ClaimRiskObjects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ClaimReserveComponents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ClaimParties",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "ClaimNumberSequences",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ClaimNumberSequences",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ClaimDocuments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "ClaimAuditLog",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ClaimAuditLog",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ClaimAuditLog",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserCreated",
                table: "ClaimAuditLog",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserModified",
                table: "ClaimAuditLog",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CauseOfLossCodes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "CauseOfLossCodes",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "CauseOfLossCodes",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CauseOfLossCodes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "CauseOfLossCodes",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserCreated",
                table: "CauseOfLossCodes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserModified",
                table: "CauseOfLossCodes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CauseOfLossCodes",
                keyColumn: "Id",
                keyValue: new Guid("30000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, null, null });

            migrationBuilder.UpdateData(
                table: "CauseOfLossCodes",
                keyColumn: "Id",
                keyValue: new Guid("30000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, null, null });

            migrationBuilder.UpdateData(
                table: "CauseOfLossCodes",
                keyColumn: "Id",
                keyValue: new Guid("30000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, null, null });

            migrationBuilder.UpdateData(
                table: "CauseOfLossCodes",
                keyColumn: "Id",
                keyValue: new Guid("30000001-0001-0001-0001-000000000004"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, null, null });

            migrationBuilder.UpdateData(
                table: "CauseOfLossCodes",
                keyColumn: "Id",
                keyValue: new Guid("30000001-0001-0001-0001-000000000005"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, null, null });

            migrationBuilder.UpdateData(
                table: "CauseOfLossCodes",
                keyColumn: "Id",
                keyValue: new Guid("30000001-0001-0001-0001-000000000006"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, null, null });

            migrationBuilder.UpdateData(
                table: "CauseOfLossCodes",
                keyColumn: "Id",
                keyValue: new Guid("30000001-0001-0001-0001-000000000007"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, null, null });

            migrationBuilder.UpdateData(
                table: "CauseOfLossCodes",
                keyColumn: "Id",
                keyValue: new Guid("30000001-0001-0001-0001-000000000008"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, null, null });

            migrationBuilder.UpdateData(
                table: "CauseOfLossCodes",
                keyColumn: "Id",
                keyValue: new Guid("30000001-0001-0001-0001-000000000009"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, null, null });

            migrationBuilder.UpdateData(
                table: "CauseOfLossCodes",
                keyColumn: "Id",
                keyValue: new Guid("30000001-0001-0001-0001-00000000000a"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, null, null });

            migrationBuilder.UpdateData(
                table: "ClaimStatusTransitions",
                keyColumn: "Id",
                keyValue: new Guid("50000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "OrganisationId", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, new Guid("11111111-1111-1111-1111-111111111111"), null, null, null });

            migrationBuilder.UpdateData(
                table: "ClaimStatusTransitions",
                keyColumn: "Id",
                keyValue: new Guid("50000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "OrganisationId", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, new Guid("11111111-1111-1111-1111-111111111111"), null, null, null });

            migrationBuilder.UpdateData(
                table: "ClaimStatusTransitions",
                keyColumn: "Id",
                keyValue: new Guid("50000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "OrganisationId", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, new Guid("11111111-1111-1111-1111-111111111111"), null, null, null });

            migrationBuilder.UpdateData(
                table: "ClaimStatusTransitions",
                keyColumn: "Id",
                keyValue: new Guid("50000001-0001-0001-0001-000000000004"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "OrganisationId", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, new Guid("11111111-1111-1111-1111-111111111111"), null, null, null });

            migrationBuilder.UpdateData(
                table: "ClaimStatusTransitions",
                keyColumn: "Id",
                keyValue: new Guid("50000001-0001-0001-0001-000000000005"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "OrganisationId", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, new Guid("11111111-1111-1111-1111-111111111111"), null, null, null });

            migrationBuilder.UpdateData(
                table: "ClaimStatusTransitions",
                keyColumn: "Id",
                keyValue: new Guid("50000001-0001-0001-0001-000000000006"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "OrganisationId", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, new Guid("11111111-1111-1111-1111-111111111111"), null, null, null });

            migrationBuilder.UpdateData(
                table: "ClaimStatusTransitions",
                keyColumn: "Id",
                keyValue: new Guid("50000001-0001-0001-0001-000000000007"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "OrganisationId", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, new Guid("11111111-1111-1111-1111-111111111111"), null, null, null });

            migrationBuilder.UpdateData(
                table: "ClaimStatusTransitions",
                keyColumn: "Id",
                keyValue: new Guid("50000001-0001-0001-0001-000000000008"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "OrganisationId", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, new Guid("11111111-1111-1111-1111-111111111111"), null, null, null });

            migrationBuilder.UpdateData(
                table: "ClaimStatusTransitions",
                keyColumn: "Id",
                keyValue: new Guid("50000001-0001-0001-0001-000000000009"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "OrganisationId", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, new Guid("11111111-1111-1111-1111-111111111111"), null, null, null });

            migrationBuilder.UpdateData(
                table: "ClaimStatusTransitions",
                keyColumn: "Id",
                keyValue: new Guid("50000001-0001-0001-0001-00000000000a"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "OrganisationId", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, new Guid("11111111-1111-1111-1111-111111111111"), null, null, null });

            migrationBuilder.UpdateData(
                table: "ClaimStatusTransitions",
                keyColumn: "Id",
                keyValue: new Guid("50000001-0001-0001-0001-00000000000b"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "OrganisationId", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, new Guid("11111111-1111-1111-1111-111111111111"), null, null, null });

            migrationBuilder.UpdateData(
                table: "ClaimStatusTransitions",
                keyColumn: "Id",
                keyValue: new Guid("50000001-0001-0001-0001-00000000000c"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "OrganisationId", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, new Guid("11111111-1111-1111-1111-111111111111"), null, null, null });

            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "Id",
                keyValue: new Guid("40000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, null, null });

            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "Id",
                keyValue: new Guid("40000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, null, null });

            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "Id",
                keyValue: new Guid("40000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, null, null });

            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "Id",
                keyValue: new Guid("40000001-0001-0001-0001-000000000004"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, null, null });

            migrationBuilder.UpdateData(
                table: "Policies",
                keyColumn: "Id",
                keyValue: new Guid("40000001-0001-0001-0001-000000000005"),
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt", "UserCreated", "UserModified" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "UserModified",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ClaimStatusTransitions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ClaimStatusTransitions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ClaimStatusTransitions");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "ClaimStatusTransitions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ClaimStatusTransitions");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "ClaimStatusTransitions");

            migrationBuilder.DropColumn(
                name: "UserModified",
                table: "ClaimStatusTransitions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ClaimNumberSequences");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ClaimNumberSequences");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ClaimAuditLog");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ClaimAuditLog");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ClaimAuditLog");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "ClaimAuditLog");

            migrationBuilder.DropColumn(
                name: "UserModified",
                table: "ClaimAuditLog");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CauseOfLossCodes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CauseOfLossCodes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CauseOfLossCodes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CauseOfLossCodes");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "CauseOfLossCodes");

            migrationBuilder.DropColumn(
                name: "UserModified",
                table: "CauseOfLossCodes");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReserveHistory",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Policies",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ClaimValidationIssues",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ClaimStatusTransitions",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ClaimRiskObjects",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ClaimReserveComponents",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ClaimParties",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ClaimDocuments",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CauseOfLossCodes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");
        }
    }
}
