namespace WebApp.Classes;

public class InAppSettings
{
    public string PageRootDirectory { get; set; }
}

public class AppSettings
{
    public InAppSettings InAppSettings { get; set; }
}
