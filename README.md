# FleetTelemetry API 

An ASP.NET Core RESTful API engineered for fleet management and continuous telemetry data ingestion. 

Designed with strict domain isolation, the architecture prioritizes data integrity, secure authentication, and memory-safe background execution.

## 🏗️ Architecture & Core Features

* **Domain-Driven Organization:** Logical and physical separation of `Fleet`, `Telemetry`, and `Identity` boundaries to ensure isolated contexts.
* **Service Layer Pattern:** Lean controllers delegating business rules to injectable services. Utilizes C# Tuples `(bool IsSuccess, string ErrorMessage, T Data)` for clean control flow, avoiding expensive exception handling for standard business logic.
* **Autonomous Background Processing:** Employs `IHostedService` for non-blocking state management (e.g., tracking overdue vehicle assignments). Safely resolves dependency injection conflicts (Singleton vs. Scoped) to interact with the database without causing Entity Framework memory leaks.
* **Query Optimization:** Implements server-side pagination (`Skip` and `Take`) to guarantee memory-safe retrieval of telemetry datasets directly at the SQL Server level (`OFFSET/FETCH`).
* **Identity & Security:** * Isolated authentication database schema (`auth`).
  * Cryptographically secure password hashing via **BCrypt**.
  * Stateless, role-based authorization using **JWT (JSON Web Tokens)** with zero clock skew tolerance.

## 🛠️ Tech Stack

* **Framework:** .NET 8 / ASP.NET Core Web API
* **Language:** C#
* **ORM:** Entity Framework Core (EF Core)
* **Database:** SQL Server
* **Security:** JWT Authentication & BCrypt.Net-Next
* **Documentation:** Scalar (Modern OpenAPI UI)

## ⚙️ Quick Start

### Prerequisites
* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* SQL Server (LocalDB or Docker container)

### Setup Instructions

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/RicardoMacedo-prj/fleet-telemetry-api.git](https://github.com/RicardoMacedo-prj/fleet-telemetry-api.git)
   cd fleet-telemetry-api
