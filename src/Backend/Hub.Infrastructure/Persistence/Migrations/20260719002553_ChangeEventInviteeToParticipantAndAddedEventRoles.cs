using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEventInviteeToParticipantAndAddedEventRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_event_requirement_completion_event_invitee_InviteeId",
                table: "event_requirement_completion");

            migrationBuilder.DropForeignKey(
                name: "FK_group_event_group_EventId",
                table: "group_event");

            migrationBuilder.DropForeignKey(
                name: "FK_event_invitee_event_EventId",
                table: "event_invitee");

            migrationBuilder.DropForeignKey(
                name: "FK_event_invitee_user_UserId",
                table: "event_invitee");

            migrationBuilder.DropColumn(
                name: "AdmissionStatus",
                table: "event_invitee");

            migrationBuilder.DropColumn(
                name: "RsvpStatus",
                table: "event_invitee");

            migrationBuilder.RenameTable(
                name: "event_invitee",
                newName: "event_participant");

            migrationBuilder.Sql(
                """
                ALTER TABLE event_participant RENAME CONSTRAINT "PK_event_invitee" TO "PK_event_participant";
                ALTER INDEX "IX_event_invitee_EventId" RENAME TO "IX_event_participant_EventId";
                ALTER INDEX "IX_event_invitee_UserId" RENAME TO "IX_event_participant_UserId";
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_event_participant_event_EventId",
                table: "event_participant",
                column: "EventId",
                principalTable: "event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_event_participant_user_UserId",
                table: "event_participant",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateTable(
                name: "event_role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsSealed = table.Column<bool>(type: "boolean", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_role_event_EventId",
                        column: x => x.EventId,
                        principalTable: "event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_participant_role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_participant_role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_participant_role_event_participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "event_participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_participant_role_event_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "event_role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_group_event_GroupId",
                table: "group_event",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_group_event_GroupId_EventId",
                table: "group_event",
                columns: new[] { "GroupId", "EventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_requirement_completion_InviteeId_RequirementId",
                table: "event_requirement_completion",
                columns: new[] { "InviteeId", "RequirementId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_participant_EventId_UserId",
                table: "event_participant",
                columns: new[] { "EventId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_participant_role_ParticipantId",
                table: "event_participant_role",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_event_participant_role_RoleId",
                table: "event_participant_role",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_event_participant_role_RoleId_ParticipantId",
                table: "event_participant_role",
                columns: new[] { "RoleId", "ParticipantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_role_EventId",
                table: "event_role",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_event_role_EventId_Name",
                table: "event_role",
                columns: new[] { "EventId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_event_requirement_completion_event_participant_InviteeId",
                table: "event_requirement_completion",
                column: "InviteeId",
                principalTable: "event_participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_group_event_group_GroupId",
                table: "group_event",
                column: "GroupId",
                principalTable: "group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_event_requirement_completion_event_participant_InviteeId",
                table: "event_requirement_completion");

            migrationBuilder.DropForeignKey(
                name: "FK_group_event_group_GroupId",
                table: "group_event");

            migrationBuilder.DropForeignKey(
                name: "FK_event_participant_event_EventId",
                table: "event_participant");

            migrationBuilder.DropForeignKey(
                name: "FK_event_participant_user_UserId",
                table: "event_participant");

            migrationBuilder.DropTable(
                name: "event_participant_role");

            migrationBuilder.DropTable(
                name: "event_role");

            migrationBuilder.DropIndex(
                name: "IX_group_event_GroupId",
                table: "group_event");

            migrationBuilder.DropIndex(
                name: "IX_group_event_GroupId_EventId",
                table: "group_event");

            migrationBuilder.DropIndex(
                name: "IX_event_requirement_completion_InviteeId_RequirementId",
                table: "event_requirement_completion");

            migrationBuilder.DropIndex(
                name: "IX_event_participant_EventId_UserId",
                table: "event_participant");

            migrationBuilder.RenameTable(
                name: "event_participant",
                newName: "event_invitee");

            migrationBuilder.Sql(
                """
                ALTER TABLE event_invitee RENAME CONSTRAINT "PK_event_participant" TO "PK_event_invitee";
                ALTER INDEX "IX_event_participant_EventId" RENAME TO "IX_event_invitee_EventId";
                ALTER INDEX "IX_event_participant_UserId" RENAME TO "IX_event_invitee_UserId";
                """);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionStatus",
                table: "event_invitee",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddColumn<string>(
                name: "RsvpStatus",
                table: "event_invitee",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddForeignKey(
                name: "FK_event_invitee_event_EventId",
                table: "event_invitee",
                column: "EventId",
                principalTable: "event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_event_invitee_user_UserId",
                table: "event_invitee",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_event_requirement_completion_event_invitee_InviteeId",
                table: "event_requirement_completion",
                column: "InviteeId",
                principalTable: "event_invitee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_group_event_group_EventId",
                table: "group_event",
                column: "EventId",
                principalTable: "group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
