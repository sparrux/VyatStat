using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedRequirementVerifiersAndVerificationsAndOthers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_organizer");

            migrationBuilder.DropTable(
                name: "event_requirement_completion");

            migrationBuilder.DropIndex(
                name: "IX_requirement_preset_VerificationMode",
                table: "requirement_preset");

            migrationBuilder.DropIndex(
                name: "IX_event_requirement_VerificationMode",
                table: "event_requirement");

            migrationBuilder.DropColumn(
                name: "IsMandatory",
                table: "requirement_preset");

            migrationBuilder.DropColumn(
                name: "VerificationMode",
                table: "requirement_preset");

            migrationBuilder.DropColumn(
                name: "IsMandatory",
                table: "event_requirement");

            migrationBuilder.DropColumn(
                name: "VerificationMode",
                table: "event_requirement");

            migrationBuilder.CreateTable(
                name: "event_requirement_assignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignParticipantId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequirementId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_requirement_assignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_requirement_assignment_event_participant_AssignPartic~",
                        column: x => x.AssignParticipantId,
                        principalTable: "event_participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_requirement_assignment_event_requirement_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "event_requirement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_requirement_verifier",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    RequirementId = table.Column<Guid>(type: "uuid", nullable: false),
                    verifier_type = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    verifier_participant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    verifier_role_id = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_requirement_verifier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_requirement_verifier_event_participant_verifier_parti~",
                        column: x => x.verifier_participant_id,
                        principalTable: "event_participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_requirement_verifier_event_requirement_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "event_requirement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_requirement_verifier_event_role_verifier_role_id",
                        column: x => x.verifier_role_id,
                        principalTable: "event_role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_requirement_verification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VerifierId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequirementAssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    verification_type = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    verified_by_participant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    verified_by_role_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_requirement_verification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_requirement_verification_event_participant_role_verif~",
                        column: x => x.verified_by_role_id,
                        principalTable: "event_participant_role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_requirement_verification_event_participant_verified_b~",
                        column: x => x.verified_by_participant_id,
                        principalTable: "event_participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_requirement_verification_event_requirement_assignment~",
                        column: x => x.RequirementAssignmentId,
                        principalTable: "event_requirement_assignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_requirement_verification_event_requirement_verifier_V~",
                        column: x => x.VerifierId,
                        principalTable: "event_requirement_verifier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_requirement_assignment_AssignParticipantId",
                table: "event_requirement_assignment",
                column: "AssignParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_event_requirement_assignment_RequirementId",
                table: "event_requirement_assignment",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_event_requirement_verification_RequirementAssignmentId",
                table: "event_requirement_verification",
                column: "RequirementAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_event_requirement_verification_verified_by_participant_id",
                table: "event_requirement_verification",
                column: "verified_by_participant_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_requirement_verification_verified_by_role_id",
                table: "event_requirement_verification",
                column: "verified_by_role_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_requirement_verification_VerifierId",
                table: "event_requirement_verification",
                column: "VerifierId");

            migrationBuilder.CreateIndex(
                name: "IX_event_requirement_verifier_RequirementId",
                table: "event_requirement_verifier",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_event_requirement_verifier_verifier_participant_id",
                table: "event_requirement_verifier",
                column: "verifier_participant_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_requirement_verifier_verifier_role_id",
                table: "event_requirement_verifier",
                column: "verifier_role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_requirement_verification");

            migrationBuilder.DropTable(
                name: "event_requirement_assignment");

            migrationBuilder.DropTable(
                name: "event_requirement_verifier");

            migrationBuilder.AddColumn<bool>(
                name: "IsMandatory",
                table: "requirement_preset",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerificationMode",
                table: "requirement_preset",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsMandatory",
                table: "event_requirement",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerificationMode",
                table: "event_requirement",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "event_organizer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_organizer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_organizer_event_EventId",
                        column: x => x.EventId,
                        principalTable: "event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_organizer_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_requirement_completion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InviteeId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequirementId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    VerificationStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_requirement_completion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_requirement_completion_event_participant_InviteeId",
                        column: x => x.InviteeId,
                        principalTable: "event_participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_requirement_completion_event_requirement_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "event_requirement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_requirement_preset_VerificationMode",
                table: "requirement_preset",
                column: "VerificationMode");

            migrationBuilder.CreateIndex(
                name: "IX_event_requirement_VerificationMode",
                table: "event_requirement",
                column: "VerificationMode");

            migrationBuilder.CreateIndex(
                name: "IX_event_organizer_EventId",
                table: "event_organizer",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_event_organizer_UserId",
                table: "event_organizer",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_event_requirement_completion_InviteeId",
                table: "event_requirement_completion",
                column: "InviteeId");

            migrationBuilder.CreateIndex(
                name: "IX_event_requirement_completion_InviteeId_RequirementId",
                table: "event_requirement_completion",
                columns: new[] { "InviteeId", "RequirementId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_requirement_completion_RequirementId",
                table: "event_requirement_completion",
                column: "RequirementId");
        }
    }
}
