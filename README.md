# FleetTelemetry API 

An ASP.NET Core RESTful API engineered for fleet management and continuous telemetry data ingestion. 

Designed with strict domain isolation, the architecture prioritizes data integrity, secure authentication, and memory-safe background execution.

## 🏗️ Architecture & Core Features

* **Domain-Driven Organization:** Logical and physical separation of `Fleet`, `Telemetry`, and `Identity` boundaries to ensure isolated contexts and maintainable database schemas.
* **Result Pattern for Flow Control:** Replaced expensive exception handling with a robust `Result<T>` pattern. This encapsulates success/failure states, standardizes API responses, and keeps controllers lean by delegating business logic to injectable services.
* **Autonomous Background Processing:** Employs `IHostedService` for non-blocking state management (e.g., tracking overdue vehicle assignments). Safely resolves scoped dependencies and utilizes EF Core's `ExecuteUpdateAsync` for high-performance, bulk SQL updates without memory overhead.
* **Query Optimization:** Implements server-side pagination (`Skip` and `Take`) to guarantee memory-safe retrieval of large telemetry datasets directly at the SQL Server level (`OFFSET/FETCH`).
* **Identity & Security:**
  * Isolated authentication database schema (`Auth`).
  * Cryptographically secure password hashing via **BCrypt**.
  * Stateless, role-based authorization using **JWT (JSON Web Tokens)** with zero clock skew tolerance.
* **Containerized Environment:** Fully dockerized setup combining the API and a pre-configured SQL Server instance, allowing for rapid deployment and consistency across environments.

## 🛠️ Tech Stack

* **Framework:** .NET 8 / ASP.NET Core Web API
* **Language:** C#
* **ORM:** Entity Framework Core (EF Core 8)
* **Database:** SQL Server (Containerized)
* **Security:** JWT Authentication & BCrypt.Net-Next
* **Documentation:** Scalar (Modern OpenAPI UI)
* **DevOps:** Docker & Docker Compose

## ⚙️ Quick Start

### Prerequisites
* [Docker Desktop](https://www.docker.com/products/docker-desktop) installed and running.
* *(Alternative)* .NET 8 SDK and a local SQL Server instance if running without Docker.

### Setup Instructions

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/RicardoMacedo-prj/fleet-telemetry-api.git](https://github.com/RicardoMacedo-prj/fleet-telemetry-api.git)
   cd fleet-telemetry-api
   ```

2. **Run with Docker Compose (Recommended):**
   ```bash
   docker-compose up --build -d
   ```
   *Note: This command orchestrates both the SQL Server database and the API container. Entity Framework Migrations and Admin user seeding are automatically applied on startup.*

3. **Access the API Documentation:**
   Once the containers are running, open your browser and navigate to the Scalar UI:
   `http://localhost:8080/scalar/v1`
