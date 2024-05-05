# Use the official .NET SDK as the base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy the .NET solution file
COPY *.sln .

# Copy and restore the NuGet packages
COPY src/ElsaMina.Console/*.csproj /app/src/ElsaMina.Console/
COPY src/ElsaMina.Core/*.csproj /app/src/ElsaMina.Core/
COPY src/ElsaMina.Commands/*.csproj /app/src/ElsaMina.Commands/
COPY src/ElsaMina.DataAccess/*.csproj /app/src/ElsaMina.DataAccess/
COPY test/ElsaMina.Test/*.csproj /app/test/ElsaMina.Test/
RUN dotnet restore

# Copy the entire solution directory
COPY . .

# Build the application
RUN dotnet build --no-restore --configuration Release

# Publish the application
RUN dotnet publish --no-restore --no-build --configuration Release --output /app/dist /app/src/ElsaMina.Console/ElsaMina.Console.csproj

# Set runtime base image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS aspnet

# Copy the published output from the build stage
COPY --from=build /app/dist .

# Set the environmnent variable
ENV ELSA_MINA_ENV="prod"

# Set the entry point for the application
ENTRYPOINT ["dotnet", "ElsaMina.Console.dll"]