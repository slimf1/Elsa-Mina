FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY *.sln .

COPY src/ElsaMina.Console/*.csproj /app/src/ElsaMina.Console/
COPY src/ElsaMina.Core/*.csproj /app/src/ElsaMina.Core/
COPY src/ElsaMina.Commands/*.csproj /app/src/ElsaMina.Commands/
COPY src/ElsaMina.DataAccess/*.csproj /app/src/ElsaMina.DataAccess/
COPY test/ElsaMina.Test/*.csproj /app/test/ElsaMina.Test/
COPY test/ElsaMina.IntegrationTests/*.csproj /app/test/ElsaMina.IntegrationTests/
RUN dotnet restore

COPY . .

RUN dotnet build --no-restore --configuration Release
RUN dotnet publish --no-restore --no-build --configuration Release --output /app/dist /app/src/ElsaMina.Console/ElsaMina.Console.csproj

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS aspnet

COPY --from=build /app/dist .

ENV ELSA_MINA_ENV="prod"
ENTRYPOINT ["dotnet", "ElsaMina.Console.dll"]