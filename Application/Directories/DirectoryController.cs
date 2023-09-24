using Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Application.Directories;

[ApiController]
[Route("api/[controller]")]
public class DirectoryController
{
    private readonly string _rootDir;
    private readonly IOptionsMonitor<InAppSettings> _settings;

    public DirectoryController(IOptionsMonitor<InAppSettings> settings)
    {
        _settings = settings;
        _rootDir = settings.CurrentValue.PageRootDirectory;
    }

    // Creates a new folder
    [NonAction]
    public IActionResult CreateFolder(string name)
    {
        Directory.CreateDirectory(name);
        return new OkResult();
    }
    
    // Overload to create additional child folders
    [HttpPost("createfolder/{name}")]

    public IActionResult CreateFolder(string name, string[]? childFolders)
    {
        var dirPath = Path.Combine(_rootDir, name);
        
        if (null == childFolders?.Length || childFolders.Length == 0)
        {
            return CreateFolder(dirPath);
        }

        CreateFolder(dirPath);
    
        foreach (var childFolder in childFolders)
        {
            CreateFolder(Path.Combine(dirPath, childFolder));
        }

        return new OkResult();
    }

    // Deletes a folder
    [HttpDelete("deletefolder/{name}")]
    public IActionResult DeleteFolder(string name)
    {
        var dirPath = Path.Combine(_rootDir, name);
        Directory.Delete(dirPath, true);
        return new OkResult();
    }

    // Lists all folders
    [HttpGet("getfolders")]
    public string ListFolders()
    {
        var dirs = Directory.GetDirectories(_rootDir);
        var folderNames = dirs.Select(Path.GetFileName).ToList();
        return JsonConvert.SerializeObject(folderNames);
    }

    // Lists all files in a folder
    [HttpGet("getfiles/{name}")]
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
    
    [HttpDelete("deletefile")]
    public IActionResult DeleteFile(string folderName, params string[] fileNames)
    {
        var dirPath = Path.Combine(_rootDir, folderName, "Uploads");

        foreach (var fileName in fileNames)
        {
            var filePath = Path.Combine(dirPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return new NotFoundResult();
            }

            System.IO.File.Delete(filePath);
        }

        return new OkResult();
    }
    
    // Uploads a file to a specified folder
    [HttpPost("uploadfile")]
    public async Task<IActionResult> UploadFile(IFormFile file, string folderName)
    {
        var dirPath = Path.Combine(_rootDir, folderName, "Uploads");
        Directory.CreateDirectory(dirPath);
    
        // Check if the file type is allowed
        if (!AllowedFileType(file.FileName, folderName))
        {
            return new UnsupportedMediaTypeResult();
        }

        var filePath = Path.Combine(dirPath, file.FileName);
    
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return new OkResult();
    }

    // Checks if the file type is allowed based on application settings
    [NonAction]
    public bool AllowedFileType(string fileName, string folderName)
    {
        var extension = Path.GetExtension(fileName).TrimStart('.').ToLower();
        if (_settings.CurrentValue.AllowedFileTypes != null)
        {
            var folder = _settings.CurrentValue.AllowedFileTypes.FirstOrDefault(f => f.FolderName == folderName);
    
            // If folder not found in settings, or no specific types are allowed, return false
            if (folder == null || folder.AllowedTypes == null || !folder.AllowedTypes.Any())
            {
                return false;
            }

            // If allowed types contains "*.*", all types are allowed
            if (folder.AllowedTypes.Contains("*.*"))
            {
                return true;
            }

            return folder.AllowedTypes.Contains($".{extension}");
        }

        return false;
    }
}