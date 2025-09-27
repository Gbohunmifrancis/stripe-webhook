# Stripe Payment Processing - Clean Architecture Solution

## Overview
A complete ASP.NET Core Clean Architecture solution that integrates with Stripe for secure payment processing. This solution implements both one-time payments and recurring subscriptions with webhook handling.


1. **Domain Layer** (`StripePayment.Domain`)
   - Core entities with strongly-typed IDs
   - Business rules and domain logic
   - No external dependencies

2. **Application Layer** (`StripePayment.Application`)
   - CQRS commands and queries with MediatR
   - Result pattern for error handling
   - Application interfaces and DTOs
   - FluentValidation for input validation

3. **Infrastructure Layer** (`StripePayment.Infrastructure`)
   - Stripe.NET service implementation
   - Entity Framework Core with SQL Server
   - Database configurations and migrations

4. **WebAPI Layer** (`StripePayment.WebAPI`)
   - RESTful API controllers
   - MediatR integration
   - Swagger documentation
   - Webhook endpoints


### ✅ Core Payment Features
- **One-Time Payments**: Create Stripe Checkout sessions for single purchases
- **Recurring Subscriptions**: Create checkout sessions for subscription-based products
- **Refund Processing**: Issue full or partial refunds programmatically
- **Secure Webhook Handling**: Process Stripe webhooks with signature verification

### ✅ Architecture Features
- **Strongly-Typed IDs**: Type-safe entity identifiers
- **CQRS Pattern**: Separation of commands and queries
- **Result Pattern**: Graceful error handling without exceptions
- **Clean Architecture**: Proper dependency inversion and separation of concerns

## API Endpoints

### Payments Controller
```http
POST /api/payments/checkout/one-time    # Create one-time payment session
POST /api/payments/checkout/subscription # Create subscription session  
POST /api/payments/refund               # Issue refund
GET  /api/payments/orders/{orderId}     # Get order details
```

### Webhooks Controller
```http
POST /api/webhooks/stripe               # Handle Stripe webhooks
GET  /api/webhooks/health               # Health check
```

## Configuration

### Database
The solution uses SQL Server LocalDB by default:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=StripePaymentDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### Stripe Configuration
Update your `appsettings.json` with your Stripe keys:
```json
{
  "Stripe": {
    "SecretKey": "sk_test_YOUR_SECRET_KEY",
    "PublishableKey": "pk_test_YOUR_PUBLISHABLE_KEY", 
    "WebhookSecret": "whsec_YOUR_WEBHOOK_SECRET",
    "SuccessUrl": "https://localhost:5001/success",
    "CancelUrl": "https://localhost:5001/cancel"
  }
}
```

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server LocalDB
- Stripe Test Account

### Running the Application

1. **Clone and Build**
   ```bash
   dotnet restore
   dotnet build
   ```

2. **Update Configuration**
   - Add your Stripe keys to `appsettings.json`
   - Database will be created automatically on first run

3. **Run the Application**
   ```bash
   cd StripePayment.WebAPI
   dotnet run
   ```

4. **Access Swagger UI**
   - Navigate to `https://localhost:5001` (or the port shown in console)
   - Explore and test the API endpoints

### Testing Webhooks

1. **Install Stripe CLI**
   ```bash
   stripe login
   stripe listen --forward-to https://localhost:5001/api/webhooks/stripe
   ```

2. **Test Webhook Events**
   ```bash
   stripe trigger checkout.session.completed
   stripe trigger invoice.payment_succeeded
   ```

## Project Structure
```
├── StripePayment.Domain/
│   ├── Common/              # Base entities and strongly-typed IDs
│   ├── Entities/            # Domain entities (Customer, Product, Order, Subscription)
│   └── Enums/               # Domain enumerations
├── StripePayment.Application/
│   ├── Commands/            # CQRS commands with handlers
│   ├── Queries/             # CQRS queries with handlers
│   ├── DTOs/                # Data transfer objects
│   ├── Interfaces/          # Application contracts
│   └── Common/              # Result pattern and errors
├── StripePayment.Infrastructure/
│   ├── Persistence/         # EF Core DbContext and configurations
│   ├── Services/            # Stripe service implementation
│   └── Configuration/       # Options and dependency injection
└── StripePayment.WebAPI/
    ├── Controllers/         # API controllers
    └── Program.cs           # Application startup






![alt text](image-1.png)
```

## Key Technologies
- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12 with nullable reference types
- **Architecture**: Clean Architecture
- **CQRS**: MediatR
- **Validation**: FluentValidation
- **Database**: Entity Framework Core with SQL Server
- **Payment**: Stripe.NET SDK
- **Documentation**: Swagger/OpenAPI

## Webhook Events Handled
- `checkout.session.completed` - Fulfills orders and activates subscriptions
- `invoice.payment_succeeded` - Handles subscription renewals

## Development Guidelines
- Uses Allman style braces and 4-space indentation
- Async/await throughout with proper CancellationToken usage
- Result pattern instead of exceptions for business logic
- Strongly-typed IDs for type safety
- File-scoped namespaces and modern C# features

## Security Features
- Webhook signature verification
- Secure Stripe key management
- HTTPS enforcement
- Input validation

---
