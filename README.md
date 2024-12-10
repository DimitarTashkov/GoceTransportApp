# Project Title

Goce Transport app is a modern web application for local transport visualization.

## Table of Contents

- [Description](#description)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)

## Description

My app offers intuitive design for transport companies, seeking advertisement and digital availability within the app. Business representatives can manage their organization bus lines, routes, schedules, drivers, vehicles and tickets they use to operate. This app should aid the accessibility to information about transport service in Goce Delchev and nearby villages. Users can reassure themselves about changes in bus schedules and routes online, without the need of contacting company individuals. It should help citizens to akcnowledge with bus working hours better.
## Technologies Used

List the major technologies, frameworks, libraries, and tools used in this project:

- **Programming Languages**: C#
- **Framework**: ASP.NET Core 8
- **Database**: SQL Server, SQLite, MSSQL
- **ORM**: Entity Framework Core
- **Frontend**: Blazor, Razor Pages, HTML, CSS, Bootstrap
- **Authentication**: ASP.NET Core Identity
- **Testing**: xUnit, NUnit,
- **Others**: Swagger, Web API

## Features

- Feature 1: Explore all existing routes, provided by organizations
- Feature 2: Explore all existing schedules, provided by organizations
- Feature 3: Explore all existing organizations
- Feature 4: Purchase tickets for routes
- Feature 5: Create organizations
- Feature 6: Manage organizations's routes, schedules, vehicles, tickets, drivers
- Feature 7: Create essentials for organizations (routes, schedules, vehicles, etc...)
- Feature 8: Full control over own organizations and their resources

## Installation

To install and set up the project locally, follow these steps:

1. Clone this repository:
   ```bash
   git clone https://github.com/DimitarTashkov/GoceTransportApp.git
   
2. Adjust connection string for local database in GoceTransport.Web.appsettings.json.

3. Configure CORS local host support. After every clone and build this application changes its localhost port, making it impossible to communicate with the Web API. Check your port and change mine in GoceTranpsort.WebAPI.appsettings.json.
