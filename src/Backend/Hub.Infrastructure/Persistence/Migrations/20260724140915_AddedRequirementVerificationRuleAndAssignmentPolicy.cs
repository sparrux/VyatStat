using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedRequirementVerificationRuleAndAssignmentPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "verifier_rule_id",
                table: "event_requirement_verifier",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignmentPolicy",
                table: "event_requirement",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "event_requirement_verification_rule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VerifierId = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_type = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_requirement_verification_rule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_requirement_verification_rule_event_requirement_verif~",
                        column: x => x.VerifierId,
                        principalTable: "event_requirement_verifier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_requirement_verification_rule_VerifierId",
                table: "event_requirement_verification_rule",
                column: "VerifierId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_requirement_verification_rule");

            migrationBuilder.DropColumn(
                name: "verifier_rule_id",
                table: "event_requirement_verifier");

            migrationBuilder.DropColumn(
                name: "AssignmentPolicy",
                table: "event_requirement");
        }
    }
}
