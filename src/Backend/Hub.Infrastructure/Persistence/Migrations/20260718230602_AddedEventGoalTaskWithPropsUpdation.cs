using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedEventGoalTaskWithPropsUpdation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State_CurrentValue",
                table: "event_goal");

            migrationBuilder.DropColumn(
                name: "State_TargetValue",
                table: "event_goal");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "event_goal",
                newName: "Name");

            migrationBuilder.CreateTable(
                name: "event_goal_task",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GoalId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_goal_task", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_goal_task_event_goal_GoalId",
                        column: x => x.GoalId,
                        principalTable: "event_goal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_goal_task_GoalId",
                table: "event_goal_task",
                column: "GoalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_goal_task");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "event_goal",
                newName: "Title");

            migrationBuilder.AddColumn<int>(
                name: "State_CurrentValue",
                table: "event_goal",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "State_TargetValue",
                table: "event_goal",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
