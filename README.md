# CarAutomotive Platform

[![CarAutomotive Backend CI](https://github.com/muhamed-husseny/CarAutomotive/actions/workflows/ci.yml/badge.svg)](https://github.com/muhamed-husseny/CarAutomotive/actions/workflows/ci.yml)

A production-ready **.NET 8 Onion Architecture** backend for a web-first automotive marketplace and workshop platform — combining a spare-parts e-commerce store with smart vehicle fitment, interactive PostGIS workshop discovery, appointment scheduling, digital repair invoicing, and Stripe split-ledger payments.

<img width="1821" height="871" alt="image" src="https://github.com/user-attachments/assets/edb5a641-110b-4e40-911b-73d4c430c7ad" />


---

## Features

### Authentication & Authorization
- User Registration & Login
- Email Verification
- Password Reset & Change Password
- Refresh Tokens
- Role-Based Authorization (Customer, Mechanic, Merchant, Admin)

### User & Garage Management
- Profile Management
- Update Personal Information
- User Garage & Vehicle Management

### Product Management & Compatibility
- Product Catalog & Brands Management
- Categories Management
- Product Images Management (Supabase Storage)
- Vehicle Compatibility Detection (Fitment Validation)
- Search by Name and Description

### Shopping Cart
- Add Items to Cart
- Update Cart Items
- Remove Items from Cart
- Clear Cart

### Order Management
- Create Orders
- Multi-Merchant Order Splitting
- View User & Merchant Orders
- Cancel Orders
- Stock Validation

### Payment & Ledger Integration
- Stripe Payment Integration
- Split-Ledger Calculation (15% Platform Commission)
- Verify Payments & Webhook Handling
- Payment & Wallet Ledger History

### Product Discovery & Caching
- Filtering by Category, Brand, and Price
- Sorting Products & Pagination
- Distributed Redis Caching (10-min TTL) with Cache Invalidation
- Output Caching (5-min TTL)

### Mechanic Services
- Spatial Mechanic Search by Location (PostGIS)
- Appointment Scheduling with Conflict Validation
- Mechanic Terminal & Repair Invoicing
- Appointment Management
- Ratings & Reviews

### Admin Features
- User Directory & Permission Management
- Merchant & Mechanic Approval Workflows
- Product & Catalog Management

---

## Technologies

- Onion Architecture
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL & PostGIS
- Redis & Output Caching
- Stripe Payment Gateway
- Supabase Storage
- JWT Authentication
- FluentValidation & AutoMapper
- xUnit & Moq (Unit, Integration, and E2E Testing)
- Docker & Docker Compose
- Swagger & Postman
- Repository, Unit of Work, and Specification Patterns

---

## Architecture

The project follows Onion Architecture principles to ensure:

- Separation of Concerns
- Maintainability
- Scalability
- Testability

---

## Project Structure

- **src/**
  - `CarAutomotive.Core` — Domain entities, enums, interfaces, specifications, shared DTOs
  - `CarAutomotive.Application` — Application services, DTOs, AutoMapper profiles, FluentValidation validators
  - `CarAutomotive.Infrastructure` — EF Core (`ApplicationDbContext`), PostGIS spatial queries, Redis caching, Stripe, Supabase, idempotent `DemoDataSeeder`
  - `CarAutomotive.API` — Versioned `/api/v1/...` controllers, JWT/RBAC auth, exception middleware, rate limiting, Swagger OpenAPI
- **tests/**
  - `CarAutomotive.Tests` — xUnit + Moq Unit, Integration, API Contract, RBAC/IDOR Security, and Seeder Idempotency tests

---

## API Overview (`/api/v1`)

| Domain | Base Route | Highlights |
| --- | --- | --- |
| **Auth & Accounts** | `/api/v1/account` | Register, Login, Refresh Token, Email Verification, Role Assignment |
| **Products & Fitment** | `/api/v1/products` | `Guid` product IDs, filtering/sorting/pagination, Redis caching, `POST /fitment-check` |
| **My Garage (Vehicles)** | `/api/v1/vehicles` | Unified customer garage contract (`GET`, `POST`) |
| **Workshops Map** | `/api/v1/mechanics` | Numeric `latitude`/`longitude`, `services`, `distanceKm`, query filters (`city`, `minRating`, `serviceName`), PostGIS `search-nearby`, `city/{city}` |
| **Appointments** | `/api/v1/appointments` | Conflict-validated scheduling with owner/mechanic/Admin IDOR protection |
| **Mechanic Portal** | `/api/v1/mechanic/bookings` | `[Authorize(Roles = "Mechanic")]` booking lifecycle & itemized repair invoicing |
| **Orders & Cart** | `/api/v1/orders`, `/api/v1/cart` | Multi-merchant order creation, cancellation, Redis shopping cart |
| **Merchant Portal** | `/api/v1/merchant/orders` | `[Authorize(Roles = "Merchant")]` order fulfillment management |
| **Payments** | `/api/v1/payments` | Stripe PaymentIntent creation & `[AllowAnonymous]` signature-verified `/webhook` |
| **Admin & Health** | `/api/v1/admin`, `/health` | Role-gated admin directory & PostgreSQL health check |

---

## Local Development, Testing & Docker

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Docker Desktop (optional, for containerized runs)
- Environment variables (or local `.env`, never committed): `ConnectionStrings__DefaultConnection`, `ConnectionStrings__Redis`, `Token__Key`, `Token__Issuer`, `Token__Audience`, `StripeSettings__SecretKey`, `Supabase__Url`, `Supabase__Key`, `Cors__AllowedOrigins__0`.

### Build & Run Tests (Offline — No Live Secrets Required)
```bash
dotnet restore CarAutomotive.sln
dotnet build CarAutomotive.sln -c Release --no-restore
dotnet test CarAutomotive.sln -c Release --no-build
```

### Run Locally with Opt-In Demo Seeding
```bash
dotnet run --project src/CarAutomotive.API -- --seed-demo
# or set SeedDemoData=true in environment variables
```
Seeding is **100% idempotent** and creates 6 Cairo & Giza workshops (Maadi, Nasr City, Dokki, Sheikh Zayed, 6th of October, Zamalek), spare parts + vehicle fitment data, and 4 demo role accounts (default password `DemoPass123!`, overridable via `DemoSeed__DefaultPassword`):
- **Customer:** `customer@carautomotive.demo`
- **Mechanic:** `mechanic@carautomotive.demo`
- **Merchant:** `merchant@carautomotive.demo`
- **Admin:** `admin@carautomotive.demo`

### Run with Docker (Non-Root Hardened Container)
```bash
docker build -t carautomotive-api:v1 .
docker compose up --build
```
The API listens on `http://localhost:8080` (health check at `/health`).

---

## Future Improvements

- **Mobile Application Integration:** Connect the RESTful API with a cross-platform mobile application to provide customers and mechanics with on-the-go access to GPS location tracking, instant booking, and spare parts shopping.
- **Real-time Notifications (SignalR):** Implement WebSocket communication to provide live updates for appointment status changes and order tracking.
- **Background Processing (Hangfire / Quartz.NET):** Introduce background jobs for automated tasks, such as sending appointment reminders or clearing abandoned shopping carts.
- **Advanced Observability & Logging:** Integrate structured logging and monitoring stacks (e.g., Serilog, ELK Stack, or Prometheus/Grafana) for better system health tracking.
- **Microservices Evolution:** Gradually decouple the E-commerce and Mechanic domains into independent microservices to scale them separately based on traffic demands.
