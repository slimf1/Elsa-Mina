# Elsa-Mina

[![deploy](https://github.com/slimf1/Elsa-Mina/actions/workflows/deploy.yml/badge.svg)](https://github.com/slimf1/Elsa-Mina/actions/workflows/deploy.yml)

[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=slimf1_Elsa-Mina&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=slimf1_Elsa-Mina)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=slimf1_Elsa-Mina&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=slimf1_Elsa-Mina)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=slimf1_Elsa-Mina&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=slimf1_Elsa-Mina)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=slimf1_Elsa-Mina&metric=bugs)](https://sonarcloud.io/summary/new_code?id=slimf1_Elsa-Mina)

> [!IMPORTANT]  
> This is a work in progress!

---

## About

Elsa-Mina is a [Showdown](https://psim.us) Chat Bot used in the [French Room](https://psim.us/fr) and in several private rooms.

## Main Features
* Custom commands
* Badge system
* User profiles
* Games (Connect Four, Guessing Game)
* Repeats
* Room Dashboard
* Teams Sharing

---

## Technical Info
The bot is written in C# and utilizes .NET 8. Docker is employed for deployment.

### Project Structure
The project structure is as follows:
* **ElsaMina.Core**: Contains the primary bot logic, the WebSocket client, resource files, and multiple services utilized in commands.
  * _Dependencies_: Autofac for Dependency Injection (DI), Newtonsoft for JSON serialization, Serilog for logging, RazorLight for Razor HTML templates, and Websocket.Client for the WebSocket connection.
  * _References_: ElsaMina.DataAccess
* **ElsaMina.DataAccess**: Contains the classes used to connect to a PostgreSQL database.
  * _Dependencies_: EF Core, PostgreSQL driver
* **ElsaMina.Commands**: Contains commands.
  * _Dependencies_: CSharp.Scripting for scripting
  * _References_: ElsaMina.Core
* **ElsaMina.Console**: Main entry point of the program, contains configuration files.
  * _References_: ElsaMina.Core, ElsaMina.Commands

### HTML Templates
For htmlboxes and htmlpages, HTML is rendered via Razor Templates using [Razor Light](https://github.com/toddams/RazorLight).
Templates are stored in the _Commands_ project. They are copied to the `Templates` directory at build time and pre-compiled on startup.

### Persistence
The bot connects to a PostgreSQL database for data persistence. EF Core is used for the ORM.
To modify the database, the following commands are utilized:
```bash
dotnet ef migrations add <Migration Name> # Creates a new migration
dotnet ef database update <Migration Name> # Applies a migration to an existing database
```

### Configuration

The bot can be configured using a config.json file, located at the root of the Console project.
Rename the example.config.json template and modify the relevant fields.
