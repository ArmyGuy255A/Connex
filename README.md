# Connex

### Database Migrations

```cmd
dotnet ef database drop --project Infrastructure --startup-project WebApp --force --verbose
dotnet ef migrations remove --project Infrastructure --startup-project WebApp --verbose
dotnet ef migrations add InitialMigration --project Infrastructure --startup-project WebApp --verbose
dotnet ef database update --project Infrastructure --startup-project WebApp --verbose
```

```cmd
```