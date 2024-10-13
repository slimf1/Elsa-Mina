# Database
## Update the database schema :

Using the Entity Framework and the dotnet CLI :
```
dotnet ef migrations add <Migration> --project src/ElsaMina.DataAccess
dotnet ef database update <Migration> --project src/ElsaMina.DataAccess --connection "connection string"
```