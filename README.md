# Elsa-Mina

[![deploy](https://github.com/SlimSeb/Elsa-Mina/actions/workflows/deploy.yml/badge.svg)](https://github.com/SlimSeb/Elsa-Mina/actions/workflows/deploy.yml)
[![test](https://github.com/SlimSeb/Elsa-Mina/actions/workflows/test.yml/badge.svg)](https://github.com/SlimSeb/Elsa-Mina/actions/workflows/test.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=slimf1_Elsa-Mina&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=slimf1_Elsa-Mina)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=slimf1_Elsa-Mina&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=slimf1_Elsa-Mina)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=slimf1_Elsa-Mina&metric=coverage)](https://sonarcloud.io/summary/new_code?id=slimf1_Elsa-Mina)

A chat bot for Pokémon Showdown, used in the French Room and private communities.

**[Live Demo](https://psim.us/fr)** • **[Report Bug](https://github.com/SlimSeb/Elsa-Mina/issues)**

## Prerequisites
- .NET 10.0 or higher
- PostgreSQL 12 or higher
- Docker (optional, for containerized deployment)

## Quick Start
1. Clone the repository
```bash
git clone https://github.com/SlimSeb/Elsa-Mina.git
cd Elsa-Mina
```

2. Configure the application
```bash
cp src/ElsaMina.Console/example.config.json src/ElsaMina.Console/config.json
# Edit config.json with your Showdown credentials and database connection.
```

3. Restore dependencies and build
```bash
./scripts/restore.sh
./scripts/build.sh
```

4. Run the bot
```bash
cd src/ElsaMina.Console
dotnet run
```

## Docker
```bash
docker build -t elsa-mina .
docker run -d \
  --name elsa-mina \
  -v $(pwd)/src/ElsaMina.Console/config.json:/app/config.json \
  elsa-mina
```

## Configuration
Config lives at `src/ElsaMina.Console/config.json`. Use the example file and set:
- Showdown credentials
- Room settings
- PostgreSQL connection string
- Optional API keys (Google Sheets, S3)

## Database
```bash
dotnet ef migrations add <MigrationName> --project src/ElsaMina.DataAccess
dotnet ef database update --project src/ElsaMina.DataAccess
dotnet ef migrations remove --project src/ElsaMina.DataAccess
```

## Testing
```bash
./scripts/test.sh
```

## Scripts
```bash
./scripts/restore.sh      # Restore NuGet packages
./scripts/build.sh        # Build the solution
./scripts/test.sh         # Run all tests
./scripts/publish.sh      # Publish for deployment
```

## Project Layout
- `src/ElsaMina.Console`: entry point and configuration
- `src/ElsaMina.Core`: bot runtime and services
- `src/ElsaMina.Commands`: command implementations
- `src/ElsaMina.DataAccess`: database layer

## License
MIT, see `LICENSE`.

## Roadmap
See `TODO.md`.
