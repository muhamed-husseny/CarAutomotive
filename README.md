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
- Role-Based Authorization

### User Management
- Profile Management
- Update Personal Information

### Product Management
- Product Catalog
- Categories Management
- Product Images Management
- Search by Name and Description

### Shopping Cart
- Add Items to Cart
- Update Cart Items
- Remove Items from Cart
- Clear Cart

### Order Management
- Create Orders
- View User Orders
- Cancel Orders
- Stock Validation

### Payment Integration
- Create Payments
- Verify Payments
- Payment History
- Webhook Handling

### Product Discovery
- Filtering by Category and Price
- Sorting Products
- Search Functionality
- Pagination

### Mechanic Services
- Mechanic Matching by Location
- Appointment Scheduling
- Appointment Management
- Ratings & Reviews

### Admin Features
- Product Management
- User Permission Management
- Image Management

---

## Technologies

- Onion Architecture
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- AutoMapper
- Swagger
- Postman
- Repository Pattern
- Unit of Work Pattern
- Specification Pattern

---

## Architecture

The project follows Onion Architecture principles to ensure:

- Separation of Concerns
- Maintainability
- Scalability
- Testability

---

## Project Structure

```text
CarAutomotive.API
CarAutomotive.Application
CarAutomotive.Core
CarAutomotive.Infrastructure
```

## Future Improvements

-   **Real-time Notifications (SignalR):** Implement WebSocket communication to provide live updates for appointment status changes and order tracking.
-   **Background Processing (Hangfire / Quartz.NET):** Introduce background jobs for automated tasks, such as sending appointment reminders or clearing abandoned shopping carts.
-   **Advanced Observability & Logging:** Integrate structured logging and monitoring stacks (e.g., Serilog, ELK Stack, or Prometheus/Grafana) for better system health tracking.
-   **Microservices Evolution:** Gradually decouple the E-commerce and Mechanic domains into independent microservices to scale them separately based on traffic demands.
-   **Comprehensive Testing Suite:** Expand the testing strategy to include Integration Testing for database queries and End-to-End (E2E) testing for critical business flows.
