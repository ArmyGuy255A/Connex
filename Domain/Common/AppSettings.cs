using System.Collections.Generic;

namespace Domain.Common
{
    public class Folder
    {
        public string? FolderName { get; set; }
        public List<string>? AllowedTypes { get; set; }
    }

    public class DatabaseSettings
    {
        public bool UseSqlite { get; set; }
    }

    public class KeyCloakSettings
    {
        public string? AdminRole { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? Issuer { get; set; }
        public string? ResponseType { get; set; }
        public bool RequireHttpsMetadata { get; set; }
    }

    public class WSFederationSettings
    {
        public string? MetadataAddress { get; set; }
        public string? Wtrealm { get; set; }
    }

    public class AuthenticationSettings
    {
        public KeyCloakSettings? KeyCloak { get; set; }
        public WSFederationSettings? WSFederation { get; set; }
    }

    public class FolderSettings
    {
        public string? RootDirectory { get; set; }
        public List<Folder>? AllowedFileTypes { get; set; }
    }

    public class ConnectionStrings
    {
        public string? SqlConnectionString { get; set; }
        public string? SqliteConnectionString { get; set; }
    }

    public class AppSettings
    {
        public ConnectionStrings? ConnectionStrings { get; set; }
        public DatabaseSettings? DatabaseSettings { get; set; }
        public FolderSettings? FolderSettings { get; set; }
        public AuthenticationSettings? Authentication { get; set; }
    }
}