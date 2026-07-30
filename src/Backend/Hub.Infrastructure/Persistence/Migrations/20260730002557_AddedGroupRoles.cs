using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedGroupRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "event_role",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "event_role",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "group_role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsSealed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_group_role_group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_member_role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_member_role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_group_member_role_group_member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "group_member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_member_role_group_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "group_role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_group_member_role_MemberId",
                table: "group_member_role",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_group_member_role_RoleId",
                table: "group_member_role",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_group_member_role_RoleId_MemberId",
                table: "group_member_role",
                columns: new[] { "RoleId", "MemberId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_group_role_GroupId",
                table: "group_role",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_group_role_GroupId_Name",
                table: "group_role",
                columns: new[] { "GroupId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "group_member_role");

            migrationBuilder.DropTable(
                name: "group_role");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "event_role");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "event_role");
        }
    }
}
