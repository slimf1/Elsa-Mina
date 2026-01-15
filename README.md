# Elsa-Mina

[![deploy](https://github.com/slimf1/Elsa-Mina/actions/workflows/deploy.yml/badge.svg)](https://github.com/slimf1/Elsa-Mina/actions/workflows/deploy.yml)
[![test](https://github.com/slimf1/Elsa-Mina/actions/workflows/test.yml/badge.svg)](https://github.com/slimf1/Elsa-Mina/actions/workflows/test.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=slimf1_Elsa-Mina&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=slimf1_Elsa-Mina)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=slimf1_Elsa-Mina&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=slimf1_Elsa-Mina)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=slimf1_Elsa-Mina&metric=coverage)](https://sonarcloud.io/summary/new_code?id=slimf1_Elsa-Mina)

A feature-rich chat bot for [Pokémon Showdown](https://psim.us) with a focus on community engagement and extensibility.

**[Live Demo](https://psim.us/fr)** • **[Report Bug](https://github.com/slimf1/Elsa-Mina/issues)** • **[Request Feature](https://github.com/slimf1/Elsa-Mina/issues)**

## About

Elsa-Mina is a sophisticated chat bot built for [Pokémon Showdown](https://psim.us), primarily deployed in the [French Room](https://psim.us/fr) and select private communities. The bot combines rich game mechanics, user engagement features, and a highly modular architecture to provide an extensible platform for interactive chat experiences.

## ✨ Features

### Core Functionality
- **Custom Commands**: Create and execute custom text commands with flexible scripting support (C# scripting)
- **Badge System**: Award and manage user badges to recognize achievements and contributions
- **User Profiles**: Store and display personalized user information and statistics
- **Interactive Games**: 
  - Connect Four with real-time board rendering
  - Guessing Game with difficulty levels
  - Arcade-style mini-games
- **Repeats**: Automated message repetition with configurable intervals
- **Room Dashboard**: Real-time HTML-based dashboard for room statistics and management
- **Teams Management**: Collaborative team creation and sharing functionality

### Developer-Friendly
- **Modular Architecture**: Clean separation of concerns with dedicated projects for commands, data access, logging, and services
- **Dependency Injection**: Built on Autofac for flexible dependency management
- **Comprehensive Logging**: Serilog-based logging for debugging and monitoring
- **Template Engine**: Razor-based HTML generation for dynamic content rendering
- **Database ORM**: Entity Framework Core for PostgreSQL integration
- **CI/CD Ready**: GitHub Actions workflows for automated testing, analysis, and deployment

## 🚀 Quick Start

### Prerequisites
- .NET 10.0 or higher
- PostgreSQL 12 or higher
- Docker (optional, for containerized deployment)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/slimf1/Elsa-Mina.git
cd Elsa-Mina
```

2. **Configure the application**
```bash
cp src/ElsaMina.Console/example.config.json src/ElsaMina.Console/config.json
# Edit config.json with your Showdown credentials and database connection
```

3. **Restore dependencies and build**
```bash
./scripts/restore.sh
./scripts/build.sh
```

4. **Run the bot**
```bash
cd src/ElsaMina.Console
dotnet run
```

### Docker Deployment

```bash
# Build the Docker image
docker build -t elsa-mina .

# Run the container
docker run -d \
  --name elsa-mina \
  -v $(pwd)/src/ElsaMina.Console/config.json:/app/config.json \
  elsa-mina
```

## 📚 Documentation

### Configuration
The bot is configured via a `config.json` file located in the Console project. Use `example.config.json` as a template and set:
- **Showdown credentials**: Username and password for bot authentication
- **Room settings**: Target rooms and moderation levels
- **Database connection**: PostgreSQL connection string
- **API keys**: Optional integrations (Google Sheets, S3, etc.)

### Project Architecture

#### **ElsaMina.Core**
Core bot logic, WebSocket client implementation, and service layer.
- **Dependencies**: Autofac, Newtonsoft.Json, Serilog, RazorLight, WebSocketClient
- **Responsibilities**: Message handling, command routing, WebSocket communication
- **Key Classes**: `Bot`, `Client`, various service implementations

#### **ElsaMina.DataAccess**
Database layer using Entity Framework Core.
- **Dependencies**: EF Core, PostgreSQL provider
- **Responsibilities**: Data persistence, migrations, model definitions
- **Database**: PostgreSQL

#### **ElsaMina.Commands**
Command implementations and handlers.
- **Dependencies**: CSharp.Scripting (for dynamic command evaluation)
- **Responsibilities**: Command parsing, execution, response generation

#### **ElsaMina.Console**
Application entry point and configuration.
- **Responsibilities**: Dependency injection setup, bot initialization, configuration loading

#### **Supporting Libraries**
- **ElsaMina.Logging**: Centralized logging configuration
- **ElsaMina.FileSharing**: S3-based file storage integration
- **ElsaMina.Sheets**: Google Sheets API integration

### Database Migrations

Entity Framework Core is used for database migrations. To manage the database:

```bash
# Create a new migration
dotnet ef migrations add <MigrationName> --project src/ElsaMina.DataAccess

# Apply migrations
dotnet ef database update --project src/ElsaMina.DataAccess

# Remove the latest migration (if not applied)
dotnet ef migrations remove --project src/ElsaMina.DataAccess
```

### HTML Templates

Dynamic HTML content (for htmlbox/htmlpage commands) is generated using Razor templates via [RazorLight](https://github.com/toddams/RazorLight).
- **Location**: `src/ElsaMina.Commands/Templates/`
- **Build Process**: Templates are automatically copied and pre-compiled on application startup
- **Usage**: Strongly-typed template models ensure type safety

## 🧪 Testing

Run the full test suite:

```bash
./scripts/test.sh
```

Tests are split into:
- **Unit Tests** (`test/ElsaMina.UnitTests`): Fast, isolated tests
- **Integration Tests** (`test/ElsaMina.IntegrationTests`): Database and service integration tests

## 🔄 CI/CD Pipeline

The project uses GitHub Actions for automated workflows:

- **Deploy** (`.github/workflows/deploy.yml`): Runs on push to `main`
  - Tests the build
  - Analyzes code quality with SonarCloud
  - Builds and publishes Docker image

- **Test** (`.github/workflows/test.yml`): Unit and integration tests
- **Code Quality** (`.github/workflows/sonar.yml`): SonarCloud analysis
- **Pull Requests** (`.github/workflows/pr.yml`): Validation for feature branches

## 📊 Code Quality

This project maintains high code quality standards:

- **SonarCloud** for continuous code analysis
- **Unit and integration tests** for comprehensive coverage
- **Build validation** in CI/CD pipelines
- **Static analysis** through roslyn analyzers

## 🛠️ Development

### Available Scripts

```bash
./scripts/restore.sh      # Restore NuGet packages
./scripts/build.sh        # Build the solution
./scripts/test.sh         # Run all tests
./scripts/publish.sh      # Publish for deployment
```

### Code Organization

- **Modular design** with clear dependency flow
- **Dependency Injection** via Autofac for loose coupling
- **Repository pattern** for data access
- **Service layer** for business logic
- **Razor templates** for dynamic content

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

Please ensure:
- All tests pass
- Code follows the project's style guidelines
- New features include appropriate tests
- Documentation is updated as needed

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙋 Support

- **Issues**: Report bugs or request features on [GitHub Issues](https://github.com/slimf1/Elsa-Mina/issues)
- **Discussions**: Start a discussion for questions or ideas
- **Live Community**: Join the [French Room](https://psim.us/fr) on Pokémon Showdown

## 🏗️ Roadmap

Current priorities and future enhancements are tracked in [TODO.md](TODO.md).

Key upcoming features include:
- Command hot-reloading
- Enhanced command options system
- Additional arcade games
- Extended community features
