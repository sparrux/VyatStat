using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RequirementsCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_group_event_invitee_requirement_completions_group_event_req~",
                table: "group_event_invitee_requirement_completions");

            migrationBuilder.AddForeignKey(
                name: "FK_group_event_invitee_requirement_completions_group_event_req~",
                table: "group_event_invitee_requirement_completions",
                column: "RequirementId",
                principalTable: "group_event_requirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_group_event_invitee_requirement_completions_group_event_req~",
                table: "group_event_invitee_requirement_completions");

            migrationBuilder.AddForeignKey(
                name: "FK_group_event_invitee_requirement_completions_group_event_req~",
                table: "group_event_invitee_requirement_completions",
                column: "RequirementId",
                principalTable: "group_event_requirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
