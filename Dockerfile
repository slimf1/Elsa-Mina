FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY *.sln .
COPY scripts/*.sh /app/scripts/
COPY src/ElsaMina.Console/*.csproj src/ElsaMina.Console/
COPY src/ElsaMina.Core/*.csproj src/ElsaMina.Core/
COPY src/ElsaMina.Commands/*.csproj src/ElsaMina.Commands/
COPY src/ElsaMina.DataAccess/*.csproj src/ElsaMina.DataAccess/
COPY src/ElsaMina.FileSharing/*.csproj src/ElsaMina.FileSharing/
COPY src/ElsaMina.Logging/*.csproj src/ElsaMina.Logging/
COPY src/ElsaMina.Sheets/*.csproj src/ElsaMina.Sheets/
COPY test/ElsaMina.UnitTests/*.csproj test/ElsaMina.UnitTests/
COPY test/ElsaMina.IntegrationTests/*.csproj test/ElsaMina.IntegrationTests/

COPY . .

RUN chmod +x /app/scripts/*.sh
RUN /app/scripts/restore.sh
RUN /app/scripts/build.sh
RUN /app/scripts/publish.sh

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/output .
ENTRYPOINT ["dotnet", "ElsaMina.Console.dll"]