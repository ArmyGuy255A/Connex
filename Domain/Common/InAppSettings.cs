namespace Domain.Common;

public class Folder
{
    public string? FolderName { get; set; }
    public List<string>? AllowedTypes { get; set; }
}

public class OIDCSettings
{
    public string? Authority { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? ResponseType { get; set; }
}

public class WSFederationSettings
{
    public string? MetadataAddress { get; set; }
    public string? Wtrealm { get; set; }
}

public class Authentication
{
    public OIDCSettings? OIDC { get; set; }
    public WSFederationSettings? WSFederation { get; set; }
}

public class InAppSettings
{
    
    private List<Folder>? _allowedFileTypes;
    
    public InAppSettings()
    {
    }

    public InAppSettings(string pageRootDirectory) : this()
    {
        PageRootDirectory = pageRootDirectory;
    }

    public string PageRootDirectory { get; set; }

    public List<Folder>? AllowedFileTypes
    {
        get => _allowedFileTypes;
        set => _allowedFileTypes = value;
    }
    
    public string? SqlConnectionString { get; set; }
    public string? SqliteConnectionString { get; set; }
    public bool UseSqlite { get; set; }
    
    public Authentication? Authentication { get; set; }
    
}

public class AppSettings
{
    public InAppSettings InAppSettings { get; set; } = null!;
}
