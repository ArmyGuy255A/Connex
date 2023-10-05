namespace Domain.Common
{
    public class Folder
    {
        public string FolderName { get; set; }
        public List<string> AllowedTypes { get; set; }

        protected Folder(string folderName, List<string> allowedTypes)
        {
            FolderName = folderName;
            AllowedTypes = allowedTypes;
        }
    }

    public class DatabaseSettings
    {
        public bool UseSqlite { get; set; }
    }

    public class KeyCloakSettings
    {
        public string AdminRole { get; set; } = "";
        public string ClientId { get; set; } = "";
        public string ClientSecret { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string ResponseType { get; set; } = "";
        public bool RequireHttpsMetadata { get; set; }
    }

    public class WsFederationSettings
    {
        public string MetadataAddress { get; set; } = "";
        public string WtRealm { get; set; } = "";
    }

    public class AuthenticationSettings
    {
        public KeyCloakSettings KeyCloak { get; set; } = new KeyCloakSettings();
        public WsFederationSettings WsFederation { get; set; } = new WsFederationSettings();
    }

    public class FolderSettings
    {
        public string RootDirectory { get; set; } = "";
        public List<Folder> AllowedFileTypes { get; set; } = new List<Folder>();
    }

    public class ConnectionStrings
    {
        public string SqlConnectionString { get; set; } = "";
        public string SqliteConnectionString { get; set; } = "";
    }

    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
        public DatabaseSettings DatabaseSettings { get; set; } = new DatabaseSettings();
        public FolderSettings FolderSettings { get; set; } = new FolderSettings();
        public AuthenticationSettings Authentication { get; set; } = new AuthenticationSettings();
    }
}