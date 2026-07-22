using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedEventReportAndEventTrainingRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "event_report",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    format = table.Column<string>(type: "text", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_report", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_report_event_EventId",
                        column: x => x.EventId,
                        principalTable: "event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_report_event_participant_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "event_participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_training_rating",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RaterId = table.Column<Guid>(type: "uuid", nullable: false),
                    RatingId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_training_rating", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_training_rating_event_EventId",
                        column: x => x.EventId,
                        principalTable: "event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_training_rating_event_participant_RaterId",
                        column: x => x.RaterId,
                        principalTable: "event_participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_training_rating_group_training_module_rating_RatingId",
                        column: x => x.RatingId,
                        principalTable: "group_training_module_rating",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_training_skill",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssessorId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_training_skill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_event_training_skill_event_EventId",
                        column: x => x.EventId,
                        principalTable: "event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_training_skill_event_participant_AssessorId",
                        column: x => x.AssessorId,
                        principalTable: "event_participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_event_training_skill_group_training_module_skill_SkillId",
                        column: x => x.SkillId,
                        principalTable: "group_training_module_skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_report_AuthorId",
                table: "event_report",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_event_report_EventId",
                table: "event_report",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_event_training_rating_EventId",
                table: "event_training_rating",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_event_training_rating_RaterId",
                table: "event_training_rating",
                column: "RaterId");

            migrationBuilder.CreateIndex(
                name: "IX_event_training_rating_RatingId",
                table: "event_training_rating",
                column: "RatingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_training_skill_AssessorId",
                table: "event_training_skill",
                column: "AssessorId");

            migrationBuilder.CreateIndex(
                name: "IX_event_training_skill_EventId",
                table: "event_training_skill",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_event_training_skill_SkillId",
                table: "event_training_skill",
                column: "SkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_report");

            migrationBuilder.DropTable(
                name: "event_training_rating");

            migrationBuilder.DropTable(
                name: "event_training_skill");
        }
    }
}
