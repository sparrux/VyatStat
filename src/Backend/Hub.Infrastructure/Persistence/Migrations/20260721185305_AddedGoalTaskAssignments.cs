using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedGoalTaskAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "event_goal_task_assignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantAssignmentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_goal_task_assignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_goal_task_assignment_event_goal_task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "event_goal_task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_goal_task_assignment_event_participant_ParticipantAss~",
                        column: x => x.ParticipantAssignmentId,
                        principalTable: "event_participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_goal_task_assignment_ParticipantAssignmentId",
                table: "event_goal_task_assignment",
                column: "ParticipantAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_event_goal_task_assignment_TaskId",
                table: "event_goal_task_assignment",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_goal_task_assignment");
        }
    }
}
