using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebApp.Classes;

namespace WebApp.Controllers;

public class FolderController
{
    private readonly string _rootDir;

    public FolderController(IOptions<InAppSettings> settings)
    {
        _rootDir = settings.Value.PageRootDirectory;
    }

    // Creates a new folder
    [HttpPost]
    public IActionResult CreateFolder(string name)
    {
        var dirPath = Path.Combine(_rootDir, name);
        Directory.CreateDirectory(dirPath);
        return new OkResult();
    }
    
    // Overload to create additional child folders
    [HttpPost]
    public IActionResult CreateFolder(string name, string[] childFolders)
    {
        var dirPath = Path.Combine(_rootDir, name);
        Directory.CreateDirectory(dirPath);
    
        foreach (var childFolder in childFolders)
        {
            Directory.CreateDirectory(Path.Combine(dirPath, childFolder));
        }

        return new OkResult();
    }

    // Deletes a folder
    [HttpDelete]
    public IActionResult DeleteFolder(string name)
    {
        var dirPath = Path.Combine(_rootDir, name);
        Directory.Delete(dirPath, true);
        return new OkResult();
    }

    // Lists all folders
    [HttpGet]
    public string ListFolders()
    {
        var dirs = Directory.GetDirectories(_rootDir);
        var folderNames = dirs.Select(Path.GetFileName).ToList();
        return JsonConvert.SerializeObject(folderNames);
    }

    // Lists all files in a folder
    [HttpGet]
    public string ListFiles(string name)
    {
        var dirPath = Path.Combine(_rootDir, name);
        var files = Directory.GetFiles(dirPath);
        var fileList = files.Select(file => new 
        {
            Name = Path.GetFileName(file),
            Size = new FileInfo(file).Length
        }).ToList();
        return JsonConvert.SerializeObject(fileList);
    }
}