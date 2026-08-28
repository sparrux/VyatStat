using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VerificationStatusReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConfirmationMode",
                table: "requirement_preset",
                newName: "VerificationMode");

            migrationBuilder.RenameIndex(
                name: "IX_requirement_preset_ConfirmationMode",
                table: "requirement_preset",
                newName: "IX_requirement_preset_VerificationMode");

            migrationBuilder.RenameColumn(
                name: "CompletionStatus",
                table: "event_requirement_completion",
                newName: "VerificationStatus");

            migrationBuilder.RenameColumn(
                name: "ConfirmationMode",
                table: "event_requirement",
                newName: "VerificationMode");

            migrationBuilder.RenameIndex(
                name: "IX_event_requirement_ConfirmationMode",
                table: "event_requirement",
                newName: "IX_event_requirement_VerificationMode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VerificationMode",
                table: "requirement_preset",
                newName: "ConfirmationMode");

            migrationBuilder.RenameIndex(
                name: "IX_requirement_preset_VerificationMode",
                table: "requirement_preset",
                newName: "IX_requirement_preset_ConfirmationMode");

            migrationBuilder.RenameColumn(
                name: "VerificationStatus",
                table: "event_requirement_completion",
                newName: "CompletionStatus");

            migrationBuilder.RenameColumn(
                name: "VerificationMode",
                table: "event_requirement",
                newName: "ConfirmationMode");

            migrationBuilder.RenameIndex(
                name: "IX_event_requirement_VerificationMode",
                table: "event_requirement",
                newName: "IX_event_requirement_ConfirmationMode");
        }
    }
}
