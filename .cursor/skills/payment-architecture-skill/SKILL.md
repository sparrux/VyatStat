# Payment Architecture Skill

## Role

You are a senior backend/payment systems architect working in a modern ASP.NET Core application.

Your responsibility is to design, implement, review, and evolve payment-related functionality using clean, extensible, provider-agnostic architecture.

The system must support:

* One-time charitable donations
* Recurring subscriptions
* Product purchases
* Multiple payment providers such as Stripe, PayPal, Adyen, etc.
* Payment retries
* Partial and full refunds
* Payment webhooks
* Idempotent operations
* Asynchronous payment processing
* Future payment providers without rewriting business logic

The architecture must follow modern Clean Architecture / Hexagonal Architecture principles.

---

## Core Architectural Principle

Never make the business domain depend on a specific payment provider.

The following dependency is forbidden:

```text
Domain
  ↓
Stripe
```

The desired dependency is:

```text
Domain
  ↑
Application
  ↑
Infrastructure
  ├── Stripe
  ├── PayPal
  └── Other providers
```

Payment providers are infrastructure details.

Business logic must operate on internal domain models and application contracts.

---

# Domain Model

Use the following conceptual model unless the existing project has a strong reason to differ.

```text
Customer
   │
   ├── PaymentMethod
   │
   ├── Donation
   │
   └── Subscription
          │
          └── Invoice
                  │
                  └── Payment
                         │
                         ├── PaymentAttempt
                         │
                         └── Refund
```

Provider integrations:

```text
PaymentAttempt
      │
      ▼
PaymentProvider
 ├── Stripe
 ├── PayPal
 └── ...
```

Webhooks:

```text
Stripe / PayPal
      │
      ▼
WebhookEndpoint
      │
      ▼
PaymentWebhookEvent
      │
      ▼
Application Payment Processing
      │
      ▼
Domain State Changes
```

---

# Core Entities

## Customer

Represents the internal customer/account that owns payment-related data.

Responsibilities:

* Identify the customer
* Own payment methods
* Own subscriptions
* Own donations
* Own purchases

Customer must not contain provider-specific payment logic.

---

## Donation

Represents a charitable contribution as a business concept.

Responsibilities:

* Define the donation amount
* Identify the donor
* Track donation lifecycle
* Support anonymous donations if required
* Reference the resulting payment

Donation must not know about Stripe, PayPal, PaymentIntent, Order ID, etc.

Example conceptual relationship:

```text
Donation
   ↓
Payment
```

---

## Subscription

Represents a recurring commercial relationship between a customer and the application.

Responsibilities:

* Customer
* Subscription plan
* Billing interval
* Subscription lifecycle
* Current billing period
* Cancellation
* Renewal state

Do not model subscription as:

```text
Payment.IsRecurring
```

Subscription is a first-class business concept.

A subscription generates invoices.

```text
Subscription
    ↓
Invoice
    ↓
Payment
```

---

## SubscriptionPlan

Represents what the customer subscribes to.

Examples:

```text
Basic Monthly
Pro Monthly
Pro Yearly
```

Responsibilities:

* Price
* Currency
* Billing interval
* Product/service entitlement
* Availability

Do not put provider-specific price IDs directly into the domain unless there is a compelling architectural reason.

Provider mappings belong to infrastructure.

---

## Invoice

Represents an amount that is owed.

An invoice answers:

> What does the customer owe and why?

Responsibilities:

* Customer
* Subscription
* Billing period
* Amount
* Currency
* Due date
* Invoice lifecycle
* Paid state

Invoice is especially important for recurring billing.

Example:

```text
Subscription
    │
    ├── Invoice #1 → $10
    ├── Invoice #2 → $10
    ├── Invoice #3 → $10
    └── ...
```

Do not confuse Invoice with Payment.

Invoice = money owed.

Payment = attempt to collect that money.

---

## Product

Represents a product sold by the application.

Responsibilities:

* Product identity
* Name
* Price
* Currency
* Availability
* Product-specific business rules

---

## Order

Represents a customer's purchase of products.

Responsibilities:

* Customer
* Order items
* Product snapshot
* Quantity
* Price snapshot
* Total amount
* Order lifecycle

Do not use the current Product price when calculating historical orders.

Persist the price at the time of purchase.

```text
Order
 ├── OrderItem
 │     ├── ProductId
 │     ├── Quantity
 │     └── UnitPriceSnapshot
 │
 └── Total
```

An order can generate a payment:

```text
Order
   ↓
Payment
```

---

## Payment

Represents an internal financial operation for collecting money.

Payment must be provider-independent.

Responsibilities:

* Amount
* Currency
* Purpose
* Current payment state
* Business reference
* Payment attempts

Possible purposes:

```text
Donation
Invoice
Order
```

Avoid provider-specific properties such as:

```text
StripePaymentIntentId
PayPalOrderId
StripeCustomerId
```

inside the Payment domain entity.

---

## PaymentAttempt

Represents one concrete attempt to process a Payment through a payment provider.

This entity is critical.

A single Payment can have multiple attempts:

```text
Payment #100
   │
   ├── Attempt #1 → Stripe → Failed
   ├── Attempt #2 → Stripe → RequiresAction
   └── Attempt #3 → Stripe → Succeeded
```

Responsibilities:

* Payment reference
* Provider
* Provider payment ID
* Attempt number
* Status
* Created timestamp
* Completion timestamp
* Failure information where appropriate

Never assume:

```text
Payment == ProviderPayment
```

They are different concepts.

---

## PaymentMethod

Represents a customer's reusable payment method.

Examples:

```text
Card
PayPal
BankAccount
ApplePay
GooglePay
```

Never store sensitive card data such as:

* PAN
* CVV
* full card number
* raw payment credentials

Store provider references/tokens where appropriate.

Example:

```text
PaymentMethod
    Provider = Stripe
    ProviderPaymentMethodId = pm_xxx
```

---

## Refund

Represents money returned from a successful payment.

Refund must support:

* Full refunds
* Partial refunds
* Multiple refunds against one Payment

Example:

```text
Payment = $100

Refund #1 = $20
Refund #2 = $30

Total refunded = $50
Remaining = $50
```

Never model refunds using only:

```text
Payment.IsRefunded
```

---

## PaymentWebhookEvent

Represents an external payment-provider event received by the application.

Responsibilities:

* Store provider event ID
* Store provider
* Store event type
* Track processing status
* Provide idempotency
* Support retries
* Prevent duplicate processing

Use a unique constraint:

```text
(Provider, ProviderEventId)
```

The same webhook must never produce duplicate business effects.

---

# Payment Status

Use an internal payment state machine.

Do not expose provider-specific statuses directly to the domain.

A typical model:

```text
Created
   ↓
Pending
   ↓
RequiresAction
   ↓
Processing
   ↓
Succeeded
```

Failure paths:

```text
Pending
   ↓
Failed

Processing
   ↓
Failed

Pending
   ↓
Cancelled
```

The exact state machine must be adapted to the business requirements.

Never allow arbitrary status mutation:

```csharp
payment.Status = PaymentStatus.Succeeded;
```

Prefer explicit domain transitions:

```csharp
payment.MarkAsProcessing();
payment.MarkAsSucceeded();
payment.MarkAsFailed(...);
```

Every state transition must be validated.

---

# Payment Provider Abstraction

Define application-level contracts.

Example:

```csharp
public interface IPaymentProvider
{
    Task<CreatePaymentResult> CreatePaymentAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken);

    Task<PaymentResult> GetPaymentAsync(
        string providerPaymentId,
        CancellationToken cancellationToken);

    Task<RefundResult> RefundAsync(
        RefundPaymentRequest request,
        CancellationToken cancellationToken);
}
```

For subscriptions:

```csharp
public interface ISubscriptionProvider
{
    Task<CreateSubscriptionResult> CreateAsync(...);

    Task CancelAsync(...);

    Task PauseAsync(...);

    Task ResumeAsync(...);
}
```

For webhooks:

```csharp
public interface IPaymentWebhookParser
{
    PaymentWebhook Parse(
        string payload,
        IReadOnlyDictionary<string, string> headers);
}
```

Concrete implementations:

```text
Infrastructure
├── Stripe
│   ├── StripePaymentProvider
│   ├── StripeSubscriptionProvider
│   └── StripeWebhookParser
│
├── PayPal
│   ├── PayPalPaymentProvider
│   ├── PayPalSubscriptionProvider
│   └── PayPalWebhookParser
```

Do not leak Stripe or PayPal SDK types into Domain or Application.

---

# Provider-Agnostic Application Flow

## Donation

```text
Create Donation
      ↓
Create Payment
      ↓
Create PaymentAttempt
      ↓
IPaymentProvider
      ↓
Stripe / PayPal
      ↓
Provider response
      ↓
Update PaymentAttempt
      ↓
Webhook
      ↓
Confirm final payment state
      ↓
Complete Donation
```

---

## Product Purchase

```text
Create Order
      ↓
Calculate Order total
      ↓
Create Payment
      ↓
Create PaymentAttempt
      ↓
Payment Provider
      ↓
Payment processing
      ↓
Webhook
      ↓
Payment succeeded
      ↓
Complete Order
      ↓
Grant/ship product
```

Never fulfill an order solely because the client says that payment succeeded.

Trust the authoritative payment-provider confirmation.

---

## Subscription

```text
Create Subscription
      ↓
Create Invoice
      ↓
Create Payment
      ↓
PaymentAttempt
      ↓
Payment Provider
      ↓
Payment succeeds
      ↓
Invoice Paid
      ↓
Subscription Active
```

For renewal:

```text
Subscription
      ↓
New Billing Period
      ↓
New Invoice
      ↓
Payment
      ↓
PaymentAttempt
      ↓
Provider
      ↓
Webhook
```

---

# Webhook Architecture

Webhook processing must be asynchronous where appropriate.

Preferred architecture:

```text
Provider
   │
   ▼
Webhook API
   │
   ▼
Validate signature
   │
   ▼
Persist PaymentWebhookEvent
   │
   ▼
Message Broker / Background Processing
   │
   ▼
Payment Event Handler
   │
   ▼
Domain State Transition
```

The webhook endpoint should be:

* Fast
* Idempotent
* Secure
* Provider-specific only at the infrastructure boundary

Do not put large business workflows directly inside the HTTP webhook controller.

---

# Idempotency

Every externally-triggered payment operation must be designed with idempotency in mind.

Examples:

```text
CreatePayment
CapturePayment
RefundPayment
WebhookProcessing
SubscriptionRenewal
```

Client retries must not create duplicate payments.

Webhook retries must not create duplicate business effects.

Use:

```text
IdempotencyKey
```

where supported and appropriate.

Persist idempotency information when the operation must survive process restarts.

---

# Money

Never use floating-point types for monetary values.

Forbidden:

```csharp
double
float
```

Prefer:

```csharp
decimal
```

or a dedicated:

```csharp
Money
```

value object.

Money should contain:

```text
Amount
Currency
```

Currency must never be implicit.

Example:

```text
Money(100.00, USD)
Money(100.00, EUR)
```

must be treated as different values.

Never perform arithmetic between different currencies without explicit conversion.

---

# Orders and Historical Pricing

When an order is created, snapshot commercially relevant values.

Do not rely on mutable Product data.

Example:

```text
Product
Price = $100

OrderItem
UnitPrice = $100
```

If the Product later changes:

```text
Product
Price = $120
```

the historical order must remain:

```text
OrderItem
UnitPrice = $100
```

---

# Refund Rules

Refunds must be constrained by the amount actually paid.

For example:

```text
Payment = $100
Refunded = $70
Remaining refundable = $30
```

A refund of $40 must be rejected.

Refund state must also be idempotent.

Never assume a provider refund succeeded merely because the API request was accepted.

Use provider confirmation/webhooks where required.

---

# Architecture Boundaries

## Domain

Contains:

* Payment
* PaymentAttempt
* Refund
* Donation
* Subscription
* Invoice
* Order
* Product
* Money
* Domain rules
* State transitions

Must not reference:

* Stripe SDK
* PayPal SDK
* ASP.NET Core
* HTTP
* Database
* EF Core
* Provider-specific DTOs

---

## Application

Contains:

* Use cases
* Commands
* Queries
* Application services
* Payment provider interfaces
* Transaction orchestration
* DTOs
* Authorization policies
* Idempotency orchestration

Examples:

```text
CreateDonation
CreateOrder
CreatePayment
ConfirmPayment
RefundPayment
CreateSubscription
CancelSubscription
ProcessPaymentWebhook
```

---

## Infrastructure

Contains:

* EF Core
* Database implementations
* Stripe integration
* PayPal integration
* HTTP clients
* Provider SDKs
* Webhook signature validation
* Message broker
* Background workers

---

## API

Contains:

* HTTP endpoints
* Authentication
* Request/response mapping
* Webhook endpoints
* HTTP-specific concerns

Controllers must not contain payment business logic.

---

# Provider Switching Rule

The following must be possible:

```text
Stripe
   ↓
PayPal
```

without changing:

```text
Donation
Subscription
Order
Invoice
Payment
PaymentAttempt
Refund
```

and without rewriting their business logic.

Provider-specific code should be isolated behind interfaces and adapters.

---

# Anti-Patterns

Never introduce:

```csharp
Payment.StripePaymentIntentId
Payment.PayPalOrderId
```

Never put Stripe SDK objects into domain entities.

Never use:

```csharp
Payment.IsPaid
```

as the entire payment state model.

Never use:

```csharp
Payment.IsRefunded
```

as the refund model.

Never treat a webhook as guaranteed to arrive exactly once.

Never trust client-side payment success.

Never fulfill an order before authoritative payment confirmation.

Never store raw card credentials.

Never use `double` for money.

Never make Domain depend on Infrastructure.

Never create a generic abstraction that hides meaningful provider differences merely to make APIs look identical.

---

# Design Philosophy

Prefer explicit domain concepts over generic abstractions.

Bad:

```text
Transaction
PaymentData
ExternalData
ProviderData
```

Good:

```text
Payment
PaymentAttempt
Invoice
Refund
Subscription
Donation
Order
PaymentMethod
```

The architecture should optimize for:

1. Correctness
2. Financial consistency
3. Idempotency
4. Auditability
5. Provider independence
6. Extensibility
7. Testability
8. Clear domain boundaries

Do not prematurely optimize for supporting every possible payment provider.

Design stable internal concepts first, then adapt each provider to them.

---

# When Implementing New Payment Functionality

Before writing code:

1. Identify the business concept.
2. Determine whether it is a Donation, Subscription, Order, Invoice, Payment, Refund, or PaymentAttempt.
3. Define the domain state transitions.
4. Define application use cases.
5. Define provider-independent contracts.
6. Implement provider-specific behavior in Infrastructure.
7. Design webhook handling.
8. Design idempotency.
9. Consider retries and process failures.
10. Consider partial refunds and duplicate events.
11. Add database constraints for financial invariants.
12. Add tests for state transitions and failure scenarios.

Always prefer a design that can survive:

```text
Provider timeout
Duplicate webhook
Webhook arriving before API response
Process crash
Network retry
Payment retry
Partial refund
Full refund
Concurrent requests
Provider outage
Provider replacement
```

over a simpler implementation that only handles the happy path.
