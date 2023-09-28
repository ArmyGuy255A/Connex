# Connex

### Identity (Keycloak)

1. Pull the Keycloak Image
2. Run the Keycloak Container

```powershell
docker pull quay.io/keycloak/keycloak:latest

docker run --name keycloak `
-p 8080:8080 `
-e KEYCLOAK_ADMIN=admin `
-e KEYCLOAK_ADMIN_PASSWORD=admin `
quay.io/keycloak/keycloak:latest `
start-dev
        
# Stop and Remove the Container
docker stop keycloak
docker start keycloak
docker rm keycloak-container
```

### Database Migrations

```bash
dotnet ef database drop --project Infrastructure --startup-project WebApp --force --verbose
dotnet ef migrations remove --project Infrastructure --startup-project WebApp --verbose
dotnet ef migrations add InitialMigration --project Infrastructure --startup-project WebApp --verbose
dotnet ef database update --project Infrastructure --startup-project WebApp --verbose
```
