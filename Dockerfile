# Use the official .NET SDK as the base image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

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
RUN dotnet build --configuration Release

# Set the working directory
WORKDIR /app/src/ElsaMina.Console

# Publish the application
RUN dotnet publish --no-restore --configuration Release --output /app/publish

# Set runtime base image
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS aspnet

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Set the environmnent variable
ENV ELSA_MINA_ENV=dev

# Set the entry point for the application
ENTRYPOINT ["dotnet", "ElsaMina.Console.dll"]