# CarAutomotive Platform

Is a full-featured automotive platform that combines an e-commerce marketplace for car spare parts with an on-demand mechanic service system.

The platform allows customers to purchase automotive products, manage orders and payments, and request repair services from nearby mechanics based on location and ratings.

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
  - `CarAutomotive.API`
  - `CarAutomotive.Application`
  - `CarAutomotive.Core`
  - `CarAutomotive.Infrastructure`
- **tests/**
  - `CarAutomotive.Tests`

---

## Future Improvements

- **Mobile Application Integration:** Connect the RESTful API with a cross-platform mobile application to provide customers and mechanics with on-the-go access to GPS location tracking, instant booking, and spare parts shopping.
- **Real-time Notifications (SignalR):** Implement WebSocket communication to provide live updates for appointment status changes and order tracking.
- **Background Processing (Hangfire / Quartz.NET):** Introduce background jobs for automated tasks, such as sending appointment reminders or clearing abandoned shopping carts.
- **Advanced Observability & Logging:** Integrate structured logging and monitoring stacks (e.g., Serilog, ELK Stack, or Prometheus/Grafana) for better system health tracking.
- **Microservices Evolution:** Gradually decouple the E-commerce and Mechanic domains into independent microservices to scale them separately based on traffic demands.
