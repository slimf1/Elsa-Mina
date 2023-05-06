# Database

- This folder contains the database file
- To create or update the database, run :
```
[Set your ELSA_MINA_DATABASE_PATH env variable]
dotnet ef migrations add <Migration>
dotnet ef database update <Migration>
```