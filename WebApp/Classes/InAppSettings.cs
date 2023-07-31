namespace WebApp.Classes;

public class Folder
{
    public string FolderName { get; set; }
    public List<string> AllowedTypes { get; set; }
}

public class InAppSettings
{
    public string PageRootDirectory { get; set; }
    public List<Folder> AllowedFileTypes { get; set; }
}

public class AppSettings
{
    public InAppSettings InAppSettings { get; set; }
}
