using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nickname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "group_events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_group_events_groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_group_members_groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_members_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "group_event_descriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Format = table.Column<int>(type: "integer", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_event_descriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_group_event_descriptions_group_events_EventId",
                        column: x => x.EventId,
                        principalTable: "group_events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_event_invitees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RsvpStatus = table.Column<int>(type: "integer", nullable: false),
                    AdmissionStatus = table.Column<int>(type: "integer", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_event_invitees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_group_event_invitees_group_events_EventId",
                        column: x => x.EventId,
                        principalTable: "group_events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_event_invitees_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "group_event_locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_event_locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_group_event_locations_group_events_EventId",
                        column: x => x.EventId,
                        principalTable: "group_events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_event_locations_locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "group_event_organizers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_event_organizers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_group_event_organizers_group_events_EventId",
                        column: x => x.EventId,
                        principalTable: "group_events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_event_organizers_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "group_event_requirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsMandatory = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_event_requirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_group_event_requirements_group_events_EventId",
                        column: x => x.EventId,
                        principalTable: "group_events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "targets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsAchieved = table.Column<bool>(type: "boolean", nullable: false),
                    CurrentValue = table.Column<int>(type: "integer", nullable: false),
                    TargetValue = table.Column<int>(type: "integer", nullable: false),
                    discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_targets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_targets_group_events_EventId",
                        column: x => x.EventId,
                        principalTable: "group_events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_event_invitee_requirement_completions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InviteeId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequirementId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompletionStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_event_invitee_requirement_completions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_group_event_invitee_requirement_completions_group_event_inv~",
                        column: x => x.InviteeId,
                        principalTable: "group_event_invitees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_event_invitee_requirement_completions_group_event_req~",
                        column: x => x.RequirementId,
                        principalTable: "group_event_requirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_group_event_descriptions_EventId",
                table: "group_event_descriptions",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_group_event_invitee_requirement_completions_InviteeId_Requi~",
                table: "group_event_invitee_requirement_completions",
                columns: new[] { "InviteeId", "RequirementId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_group_event_invitee_requirement_completions_RequirementId",
                table: "group_event_invitee_requirement_completions",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_group_event_invitees_EventId",
                table: "group_event_invitees",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_group_event_invitees_UserId_EventId",
                table: "group_event_invitees",
                columns: new[] { "UserId", "EventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_group_event_locations_EventId",
                table: "group_event_locations",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_group_event_locations_LocationId",
                table: "group_event_locations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_group_event_organizers_EventId",
                table: "group_event_organizers",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_group_event_organizers_UserId_EventId",
                table: "group_event_organizers",
                columns: new[] { "UserId", "EventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_group_event_requirements_EventId_SortOrder",
                table: "group_event_requirements",
                columns: new[] { "EventId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_group_events_GroupId",
                table: "group_events",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_group_events_GroupId_StartDate",
                table: "group_events",
                columns: new[] { "GroupId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_group_events_State",
                table: "group_events",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_group_members_GroupId",
                table: "group_members",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_group_members_UserId_GroupId",
                table: "group_members",
                columns: new[] { "UserId", "GroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_targets_EventId",
                table: "targets",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_users_Nickname",
                table: "users",
                column: "Nickname",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "group_event_descriptions");

            migrationBuilder.DropTable(
                name: "group_event_invitee_requirement_completions");

            migrationBuilder.DropTable(
                name: "group_event_locations");

            migrationBuilder.DropTable(
                name: "group_event_organizers");

            migrationBuilder.DropTable(
                name: "group_members");

            migrationBuilder.DropTable(
                name: "targets");

            migrationBuilder.DropTable(
                name: "group_event_invitees");

            migrationBuilder.DropTable(
                name: "group_event_requirements");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "group_events");

            migrationBuilder.DropTable(
                name: "groups");
        }
    }
}
