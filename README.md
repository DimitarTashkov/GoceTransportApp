<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-blueviolet?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/ASP.NET_Core-MVC-blue?style=for-the-badge&logo=dotnet&logoColor=white" alt="ASP.NET Core MVC" />
  <img src="https://img.shields.io/badge/EF_Core-SQL_Server-orange?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="EF Core + SQL Server" />
  <img src="https://img.shields.io/badge/Bootstrap-5-purple?style=for-the-badge&logo=bootstrap&logoColor=white" alt="Bootstrap 5" />
  <img src="https://img.shields.io/badge/SignalR-Real--Time-red?style=for-the-badge&logo=dotnet&logoColor=white" alt="SignalR" />
  <img src="https://img.shields.io/badge/Google_Maps-API-4285F4?style=for-the-badge&logo=googlemaps&logoColor=white" alt="Google Maps" />
  <img src="https://img.shields.io/badge/Stripe-Payments-635bff?style=for-the-badge&logo=stripe&logoColor=white" alt="Stripe" />
</p>

<h1 align="center">🚍 GoceTransportApp</h1>

<p align="center">
  <strong>A multi-tenant SaaS platform connecting bus passengers with transport companies.</strong><br/>
  Search routes. Buy tickets. Manage your fleet. All in one place.
</p>

---

## 📖 Table of Contents

- [About the Project](#-about-the-project)
- [Key Features](#-key-features)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Getting Started](#-getting-started)
- [Test Accounts](#-test-accounts)
- [Testing](#-testing)
- [Localization](#-localization)
- [Roadmap](#-roadmap)
- [Contributing](#-contributing)

---

## 🎯 About the Project

**GoceTransportApp** is a full-featured web platform built with ASP.NET Core MVC that solves two problems at once:

- 🧳 **For Passengers** — An intuitive way to search bus routes, compare schedules, and purchase tickets online, no phone calls or guesswork needed.
- 🏢 **For Transport Companies** — A complete back-office system to manage fleets, drivers, routes, schedules, and ticket sales, all from a single organization dashboard.

The platform operates on a **multi-tenant model**: each transport company registers as an organization, manages its own resources independently, and is visible to all passengers browsing the platform. A tiered SaaS subscription model (Free / Starter / Pro / Enterprise) — powered by Stripe — controls how many organizations a user can operate and which premium features they can access.

---

## ✨ Key Features

### 👤 For Passengers
| Feature | Description |
|---------|-------------|
| 🔍 **Route & Schedule Search** | Find trips by origin city, destination, and date with advanced filtering and sorting |
| 🎫 **Ticket Purchase** | Buy tickets with automatic seat capacity validation |
| 📊 **Personal Dashboard** | View upcoming trips, travel history, and favorite companies at a glance |
| ⭐ **Favorites** | Follow your preferred transport companies for quick access |
| ✍️ **Reviews & Ratings** | Rate organizations after completing a trip (1–5 stars + comments) |
| 🔔 **Real-Time Notifications** | Instant toast alerts for live schedule status changes, new reviews, and ticket purchases via SignalR |
| 📬 **Notification Inbox** | Persistent bell icon in the navbar — unread badge, inbox dropdown, one-click mark-as-read |
| 🗺️ **Stop Navigation** | View stop locations on an interactive Google Maps modal and get turn-by-turn navigation directions |
| 🌙 **Dark Mode** | Toggle between light and dark theme — preference is preserved across sessions |
| 🌐 **Bilingual UI** | Switch between 🇬🇧 English and 🇧🇬 Bulgarian with one click |

### 🏢 For Transport Companies (Organizations)
| Feature | Description |
|---------|-------------|
| 🚌 **Fleet Management** | Register and manage vehicles with capacity tracking |
| 👨‍✈️ **Driver Management** | Maintain your driver roster linked to your organization |
| 🗺️ **Route Builder** | Create routes between cities with Google Maps coordinate picker for exact stop locations |
| 📅 **Schedule Generator** | Set up recurring schedules with departure/arrival times and pricing |
| 📡 **Live Status Broadcasting** | Push real-time status updates (delays, cancellations) to all passengers viewing that schedule *(Pro/Sponsor)* |
| 📈 **Organization Dashboard** | Real-time metrics — active routes, vehicles, drivers, and tickets sold |
| 🎟️ **Passenger Lists** | View boarded passengers per schedule with boarding toggle |

### ⚙️ Admin & System
| Feature | Description |
|---------|-------------|
| 💳 **Stripe Subscriptions** | Integrated Stripe Checkout for recurring SaaS billing — Starter, Pro, and Enterprise tiers with automatic webhook handling |
| 🏷️ **Membership Tiers** | Free (1 org), Pro (3 orgs), Enterprise (unlimited) — controls resource limits and premium feature access |
| 🔒 **Premium Feature Gating** | Backend nulls restricted data for Free users; frontend shows upgrade prompts — double-layer protection |
| 🔐 **Role-Based Access** | ASP.NET Identity with Admin, Organization Owner, and Passenger roles |
| 🌱 **Auto-Seeding** | `TestScenarioSeeder` populates the database with realistic test data on first run |
| 📧 **Email Notifications** | MailKit/Brevo SMTP integration for ticket confirmations and cancellations |
| 📬 **Contact Form** | Built-in contact form; admins receive a real-time SignalR alert on every new submission |
| 🔎 **SEO** | `robots.txt`, `sitemap.xml`, Open Graph & Twitter Card meta tags, per-page descriptions |
| 🛡️ **GDPR** | Full privacy policy (data collected, legal basis, retention, subject rights) and Terms of Service page |

---


## 🛠️ Tech Stack

| Layer | Technology |
|-------|------------|
| **Backend** | C# / ASP.NET Core 8 MVC |
| **Database** | SQL Server + Entity Framework Core |
| **Frontend** | Bootstrap 5, HTML5, JavaScript |
| **Authentication** | ASP.NET Core Identity (Cookie-based) |
| **Real-Time** | ASP.NET Core SignalR |
| **Maps** | Google Maps JavaScript API (coordinate picker + interactive modal) |
| **Object Mapping** | AutoMapper (custom `IMapFrom<T>` / `IMapTo<T>` interfaces) |
| **Payments** | Stripe API (Checkout Sessions, Webhooks, Subscriptions) |
| **Email** | MailKit + Brevo SMTP |
| **Logging** | Serilog (structured logging to file) |
| **Resilience** | Polly (retry + circuit breaker policies for HTTP clients) |
| **E2E Testing** | Playwright + NUnit |
| **API Docs** | Swagger / Swashbuckle (WebApi project) |

---

## 🏗️ Architecture

The solution follows a **Clean Architecture** with the **Repository Pattern** and clean separation of concerns across **16 projects**:

```
GoceTransportApp/
├── Web/
│   ├── GoceTransportApp.Web                 # ASP.NET Core MVC (main entry point)
│   ├── GoceTransportApp.Web.Infrastructure  # Filters, extensions, middleware
│   └── GoceTransportApp.Web.ViewModels      # View models & input models
├── Services/
│   ├── GoceTransportApp.Services            # Business logic contracts
│   ├── GoceTransportApp.Services.Data       # Service implementations (EF queries)
│   ├── GoceTransportApp.Services.Mapping    # AutoMapper profiles & interfaces
│   └── GoceTransportApp.Services.Messaging  # Email (MailKit/Brevo) + templates
├── Data/
│   ├── GoceTransportApp.Data                # DbContext, migrations, seeders
│   ├── GoceTransportApp.Data.Models         # Entity models (EF Code-First)
│   └── GoceTransportApp.Data.Common         # Base models, repositories
├── Common/
│   └── GoceTransportApp.Common              # Constants, enums, shared utilities
├── API/
│   └── GoceTransportApp.WebApi              # RESTful API with Swagger
└── Tests/
    ├── GoceTransportApp.Services.Data.Tests # Unit tests for services
    ├── GoceTransportApp.Web.Tests           # Unit tests for controllers
    ├── GoceTransportApp.E2ETests            # Playwright E2E browser tests
    └── Sandbox                              # Experimental / scratch project
```

---

## 🚀 Getting Started

### Prerequisites

| Tool | Version | Download |
|------|---------|----------|
| .NET SDK | 8.0+ | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0) |
| SQL Server | 2019+ (or LocalDB) | [SQL Server Downloads](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) |
| Node.js | 18+ *(only for E2E tests)* | [nodejs.org](https://nodejs.org/) |

### Step 1 — Clone the Repository

```bash
git clone https://github.com/DimitarTashkov/GoceTransportApp.git
cd GoceTransportApp
```

### Step 2 — Configure the Database

Open `Web/GoceTransportApp.Web/appsettings.json` and update the connection string to match your local SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=GoceTransportApp;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### Step 3 — (Optional) Add a Google Maps API Key

To enable the interactive stop location picker and map modal, add your key to `appsettings.json`:

```json
"GoogleMaps": {
  "ApiKey": "YOUR_API_KEY_HERE"
}
```

> Without a key the map sections are hidden for Free-tier users. Pro/Sponsor users will see a placeholder until a valid key is provided.

### Step 4 — (Optional) Configure Stripe Payments

To enable subscription checkout (Starter / Pro / Enterprise plans), add your Stripe keys to `appsettings.json`:

```json
"Stripe": {
  "PublishableKey": "pk_test_YOUR_PUBLISHABLE_KEY",
  "SecretKey": "sk_test_YOUR_SECRET_KEY",
  "StarterPriceId": "price_YOUR_STARTER_ID",
  "ProPriceId": "price_YOUR_PRO_ID",
  "EnterprisePriceId": "price_YOUR_ENTERPRISE_ID"
}
```

> Without Stripe keys the app runs in Free-tier mode. Subscription features activate once valid keys are provided.

### Step 5 — Apply Migrations & Seed Data

Using the **Package Manager Console** (Visual Studio):
```
Update-Database
```

Or via CLI:
```bash
dotnet ef database update --project Web/GoceTransportApp.Web
```

> 💡 **Auto-Seeding:** On the first run, the `TestScenarioSeeder` automatically populates the database with **3 transport companies**, **3 cities** (Sofia, Plovdiv, Varna), **routes, vehicles, drivers, schedules**, and **a sample ticket** — so you can explore the app immediately without manual data entry.

### Step 6 — Run the Application

```bash
cd Web/GoceTransportApp.Web
dotnet run
```

The app will start at `https://localhost:5001` (or the port shown in your terminal).

---

## 🔑 Test Accounts

The `TestScenarioSeeder` creates the following accounts (password for all: **`Test1234!`**):

| Email | Role | Description |
|-------|------|-------------|
| `org1@test.com` | Organization Owner | Owns **Express Lines** (Sofia → Plovdiv) |
| `org2@test.com` | Organization Owner | Owns **Global Trans** (Plovdiv → Varna) |
| `org3@test.com` | Organization Owner | Owns **Eco Travel** (Varna → Sofia) |
| `passenger@test.com` | Passenger | Has a ticket on the Express Lines route |

> Each organization comes pre-loaded with 1 vehicle (Mercedes Travego, 50 seats), 1 driver, 1 route, and 1 schedule departing 7 days from the seed date.

---

## 🧪 Testing

### E2E Tests (Playwright)

The project includes end-to-end browser tests covering organization CRUD, vehicle/route/schedule creation, ticket checkout, and more.

```bash
# Install Playwright browsers (first time only)
pwsh Tests/GoceTransportApp.E2ETests/bin/Debug/net10.0/playwright.ps1 install

# Run E2E tests
dotnet test Tests/GoceTransportApp.E2ETests
```

### Unit Tests

```bash
dotnet test Tests/GoceTransportApp.Services.Data.Tests
dotnet test Tests/GoceTransportApp.Web.Tests
```

**Test Coverage:**
- ✅ Organization CRUD + validation
- ✅ Vehicle, Route, and Schedule creation
- ✅ Ticket purchase and passenger management
- ✅ Login and authentication flows

---

## 🌐 Localization

The entire UI is available in **English (en-US)** and **Bulgarian (bg-BG)**, covering **55+ view files** with matching `.resx` resource files.

- Toggle language via the 🇬🇧 / 🇧🇬 flag dropdown in the navigation bar
- Language preference is saved in a cookie (persists for 1 year)
- Default language: **English**

**Localized areas:** Navigation, Home/Landing page, Organizations, Schedules, Tickets, Routes, Vehicles, Drivers, Cities, Contact Form, Login/Register, Dashboards, Notifications, and more.

---

## 🤝 Contributing

Contributions are welcome! If you'd like to help improve GoceTransportApp:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

> Please make sure your code compiles (`dotnet build`) and existing tests pass before submitting.


---

<p align="center">
  Made with ❤️ in Bulgaria
  <br/>
  <sub>Built with ASP.NET Core 8 &bull; Entity Framework Core &bull; Bootstrap 5 &bull; SignalR</sub>
</p>
