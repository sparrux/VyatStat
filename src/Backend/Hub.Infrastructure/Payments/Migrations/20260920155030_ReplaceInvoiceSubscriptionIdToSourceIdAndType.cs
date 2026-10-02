using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Infrastructure.Payments.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceInvoiceSubscriptionIdToSourceIdAndType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoice_subscription_SubscriptionId",
                schema: "payments",
                table: "invoice");

            migrationBuilder.RenameColumn(
                name: "SubscriptionId",
                schema: "payments",
                table: "invoice",
                newName: "SourceId");

            migrationBuilder.RenameIndex(
                name: "IX_invoice_SubscriptionId",
                schema: "payments",
                table: "invoice",
                newName: "IX_invoice_SourceId");

            migrationBuilder.AddColumn<string>(
                name: "SourceType",
                schema: "payments",
                table: "invoice",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceType",
                schema: "payments",
                table: "invoice");

            migrationBuilder.RenameColumn(
                name: "SourceId",
                schema: "payments",
                table: "invoice",
                newName: "SubscriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_invoice_SourceId",
                schema: "payments",
                table: "invoice",
                newName: "IX_invoice_SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_invoice_subscription_SubscriptionId",
                schema: "payments",
                table: "invoice",
                column: "SubscriptionId",
                principalSchema: "payments",
                principalTable: "subscription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
