# VenusERP_API

A RESTful API backend for the Venus ERP system, built with ASP.NET Core.

## Overview

VenusERP_API provides the server-side functionality for enterprise resource planning, including real-time communication, invoice management (with ZATCA compliance), and reporting capabilities.

## Tech Stack

- **Framework**: ASP.NET Core
- **Language**: C#
- **Real-time**: SignalR (RealTimeHub)
- **Compliance**: ZATCA e-invoicing support

## Project Structure




## Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (version specified in `.csproj`)
- SQL Server or compatible database

### Configuration
1. Clone the repository
2. Update `appsettings.json` with your database connection string
3. Run migrations: `dotnet ef database update`
4. Start the application: `dotnet run`

## API Documentation

When running locally, Swagger UI is available at:



## Features

- **Real-time updates** via SignalR hubs
- **ZATCA-compliant** e-invoicing for Saudi Arabia
- **Reporting engine** for business intelligence
- **RESTful API** with standard HTTP methods



