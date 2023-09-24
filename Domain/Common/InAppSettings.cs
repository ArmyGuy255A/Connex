namespace Domain.Common;

public class Folder
{
    public string? FolderName { get; set; }
    public List<string>? AllowedTypes { get; set; }
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
    
    public string? MongoDbConnectionString { get; set; }
    
}

public class AppSettings
{
    public InAppSettings InAppSettings { get; set; } = null!;
}
