# Connex

### Database Migrations

```cmd
dotnet ef migrations remove --project Infrastructure --startup-project WebApp --verbose
```

```cmd
dotnet ef migrations add InitialMigration --project Infrastructure --startup-project WebApp --verbose
```

```cmd
dotnet ef database update --project Infrastructure --startup-project WebApp --verbose
```

```cmd
dotnet ef database drop --project Infrastructure --startup-project WebApp --force --verbose
```