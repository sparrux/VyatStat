using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Infrastructure.Payments.Migrations
{
    /// <inheritdoc />
    public partial class InitialPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "payments");

            migrationBuilder.CreateTable(
                name: "customer",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "payment_webhook_event",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProviderEventId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProcessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_webhook_event", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "product",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", unicode: false, maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => x.Id);
                    table.CheckConstraint("ck_product_price_positive", "\"Price\" > 0");
                });

            migrationBuilder.CreateTable(
                name: "subscription_plan",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BillingInterval = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EntitlementKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", unicode: false, maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_plan", x => x.Id);
                    table.CheckConstraint("ck_subscription_plan_price_positive", "\"Price\" > 0");
                });

            migrationBuilder.CreateTable(
                name: "payment",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Purpose = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SucceededAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CancelledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", unicode: false, maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.Id);
                    table.CheckConstraint("ck_payment_amount_positive", "\"Amount\" > 0");
                    table.ForeignKey(
                        name: "FK_payment_customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "payments",
                        principalTable: "customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payment_method",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProviderPaymentMethodId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Brand = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastFour = table.Column<string>(type: "character varying(4)", unicode: false, maxLength: 4, nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_method", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payment_method_customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "payments",
                        principalTable: "customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscription",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanNameSnapshot = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BillingInterval = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CancelledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PausedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PeriodEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PeriodStart = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PriceSnapshot = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", unicode: false, maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subscription_customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "payments",
                        principalTable: "customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_subscription_subscription_plan_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "payments",
                        principalTable: "subscription_plan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "donation",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsAnonymous = table.Column<bool>(type: "boolean", nullable: false),
                    ReferenceType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", unicode: false, maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_donation", x => x.Id);
                    table.CheckConstraint("ck_donation_amount_positive", "\"Amount\" > 0");
                    table.ForeignKey(
                        name: "FK_donation_customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "payments",
                        principalTable: "customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_donation_payment_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "payments",
                        principalTable: "payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Total = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", unicode: false, maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order", x => x.Id);
                    table.CheckConstraint("ck_order_total_positive", "\"Total\" > 0");
                    table.ForeignKey(
                        name: "FK_order_customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "payments",
                        principalTable: "customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_payment_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "payments",
                        principalTable: "payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payment_attempt",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProviderPaymentId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AttemptNumber = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailureCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FailureMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_attempt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payment_attempt_payment_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "payments",
                        principalTable: "payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refund",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProviderRefundId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", unicode: false, maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refund", x => x.Id);
                    table.CheckConstraint("ck_refund_amount_positive", "\"Amount\" > 0");
                    table.ForeignKey(
                        name: "FK_refund_payment_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "payments",
                        principalTable: "payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invoice",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    DueDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", unicode: false, maxLength: 3, nullable: false),
                    PeriodEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PeriodStart = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice", x => x.Id);
                    table.CheckConstraint("ck_invoice_amount_positive", "\"Amount\" > 0");
                    table.ForeignKey(
                        name: "FK_invoice_customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "payments",
                        principalTable: "customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_invoice_payment_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "payments",
                        principalTable: "payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_invoice_subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "payments",
                        principalTable: "subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_item",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    LineTotalCurrency = table.Column<string>(type: "character varying(3)", unicode: false, maxLength: 3, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitPriceCurrency = table.Column<string>(type: "character varying(3)", unicode: false, maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_item", x => x.Id);
                    table.CheckConstraint("ck_order_item_quantity_positive", "\"Quantity\" > 0");
                    table.CheckConstraint("ck_order_item_unit_price_positive", "\"UnitPrice\" > 0");
                    table.ForeignKey(
                        name: "FK_order_item_order_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "payments",
                        principalTable: "order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_item_product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "payments",
                        principalTable: "product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_donation_CustomerId",
                schema: "payments",
                table: "donation",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_donation_PaymentId",
                schema: "payments",
                table: "donation",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_donation_ReferenceType_ReferenceId",
                schema: "payments",
                table: "donation",
                columns: new[] { "ReferenceType", "ReferenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_donation_Status",
                schema: "payments",
                table: "donation",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_CustomerId",
                schema: "payments",
                table: "invoice",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_DueDate",
                schema: "payments",
                table: "invoice",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_PaymentId",
                schema: "payments",
                table: "invoice",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_Status",
                schema: "payments",
                table: "invoice",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_SubscriptionId",
                schema: "payments",
                table: "invoice",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_order_CustomerId",
                schema: "payments",
                table: "order",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_order_PaymentId",
                schema: "payments",
                table: "order",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_order_Status",
                schema: "payments",
                table: "order",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_OrderId",
                schema: "payments",
                table: "order_item",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_ProductId",
                schema: "payments",
                table: "order_item",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_CustomerId",
                schema: "payments",
                table: "payment",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_IdempotencyKey",
                schema: "payments",
                table: "payment",
                column: "IdempotencyKey",
                unique: true,
                filter: "\"IdempotencyKey\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_payment_Purpose_ReferenceId",
                schema: "payments",
                table: "payment",
                columns: new[] { "Purpose", "ReferenceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_Status",
                schema: "payments",
                table: "payment",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_payment_attempt_PaymentId",
                schema: "payments",
                table: "payment_attempt",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_attempt_PaymentId_AttemptNumber",
                schema: "payments",
                table: "payment_attempt",
                columns: new[] { "PaymentId", "AttemptNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_attempt_Provider_ProviderPaymentId",
                schema: "payments",
                table: "payment_attempt",
                columns: new[] { "Provider", "ProviderPaymentId" },
                unique: true,
                filter: "\"ProviderPaymentId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_payment_method_CustomerId",
                schema: "payments",
                table: "payment_method",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_method_CustomerId_Provider_ProviderPaymentMethodId",
                schema: "payments",
                table: "payment_method",
                columns: new[] { "CustomerId", "Provider", "ProviderPaymentMethodId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_webhook_event_Provider_ProviderEventId",
                schema: "payments",
                table: "payment_webhook_event",
                columns: new[] { "Provider", "ProviderEventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_webhook_event_Status",
                schema: "payments",
                table: "payment_webhook_event",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_product_IsAvailable",
                schema: "payments",
                table: "product",
                column: "IsAvailable");

            migrationBuilder.CreateIndex(
                name: "IX_product_Name",
                schema: "payments",
                table: "product",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_refund_PaymentId",
                schema: "payments",
                table: "refund",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_refund_ProviderRefundId",
                schema: "payments",
                table: "refund",
                column: "ProviderRefundId",
                unique: true,
                filter: "\"ProviderRefundId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_CustomerId",
                schema: "payments",
                table: "subscription",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_PlanId",
                schema: "payments",
                table: "subscription",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_Status",
                schema: "payments",
                table: "subscription",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plan_EntitlementKey",
                schema: "payments",
                table: "subscription_plan",
                column: "EntitlementKey");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plan_IsAvailable",
                schema: "payments",
                table: "subscription_plan",
                column: "IsAvailable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "donation",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "invoice",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "order_item",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "payment_attempt",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "payment_method",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "payment_webhook_event",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "refund",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "subscription",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "order",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "product",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "subscription_plan",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "payment",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "customer",
                schema: "payments");
        }
    }
}
