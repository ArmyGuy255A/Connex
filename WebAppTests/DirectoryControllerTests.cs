using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using WebApp.AutoMapper;
using WebApp.Classes;
using WebApp.Controllers;

namespace WebAppTests;

public class DirectoryControllerTests
{
    private IConfiguration _config;
    private ServiceProvider _serviceProvider;

    [SetUp]
    public void Setup()
    {
        var projectDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(projectDir, "appsettings.json");

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(configPath)
            .Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.Configure<InAppSettings>(configuration.GetSection("InAppSettings"));
        serviceCollection.AddTransient<DirectoryController>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
        
        // var projectDir = Directory.GetCurrentDirectory();
        // var configPath = Path.Combine(projectDir, "appsettings.json");
        //
        // _config = new ConfigurationBuilder()
        //     .AddJsonFile(configPath)
        //     .Build();
    }

    [Test]
    public void TestCreateFolder()
    {
        var controller = _serviceProvider.GetRequiredService<DirectoryController>();
        var result = controller.CreateFolder("TestFolder");

        var rootDir = _serviceProvider.GetRequiredService<IOptions<InAppSettings>>().Value.PageRootDirectory;
        Assert.IsTrue(Directory.Exists(Path.Combine(rootDir, "TestFolder")));
        Assert.IsInstanceOf<OkResult>(result);
    }

    [Test]
    public void TestCreateFolderWithChildren()
    {
        var controller = _serviceProvider.GetRequiredService<DirectoryController>();
        var result = controller.CreateFolder("TestFolder", new[] { "Child1", "Child2" });

        var rootDir = _serviceProvider.GetRequiredService<IOptions<InAppSettings>>().Value.PageRootDirectory;
        Assert.IsTrue(Directory.Exists(Path.Combine(rootDir, "TestFolder")));
        Assert.IsTrue(Directory.Exists(Path.Combine(rootDir, "TestFolder", "Child1")));
        Assert.IsTrue(Directory.Exists(Path.Combine(rootDir, "TestFolder", "Child2")));
        Assert.IsInstanceOf<OkResult>(result);
    }

    [Test]
    public void TestListFiles()
    {
        var rootDir = _serviceProvider.GetRequiredService<IOptions<InAppSettings>>().Value.PageRootDirectory;

        // Prepare a test directory and files
        var testDir = "TestFolder";
        Directory.CreateDirectory(Path.Combine(rootDir, testDir));
        File.WriteAllText(Path.Combine(rootDir, testDir, "file1.txt"), "Hello, World!");
        File.WriteAllText(Path.Combine(rootDir, testDir, "file2.txt"), "Hello, OpenAI!");

        var controller = _serviceProvider.GetRequiredService<DirectoryController>();

        // Call the method and deserialize the result
        var resultJson = controller.ListFiles(testDir);
        var result = JsonConvert.DeserializeObject<List<FileInformation>>(resultJson);


        // Assert that the correct number of files were returned
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));

        // Assert that the names and sizes of the files are correct
        Assert.That(result[0].Name, Is.EqualTo("file1.txt"));
        Assert.That(result[1].Name, Is.EqualTo("file2.txt"));
        Assert.That(result[0].Size, Is.EqualTo(13));
        Assert.That(result[1].Size, Is.EqualTo(14));
    }
    
    [Test]
    public void TestDeleteFolder()
    {
        var rootDir = _serviceProvider.GetRequiredService<IOptions<InAppSettings>>().Value.PageRootDirectory;

        // Prepare a test directory
        var testDir = "TestFolder";
        Directory.CreateDirectory(Path.Combine(rootDir, testDir));

        var controller = _serviceProvider.GetRequiredService<DirectoryController>();

        // Call the method
        controller.DeleteFolder(testDir);

        // Assert that the directory no longer exists
        Assert.That(Directory.Exists(Path.Combine(rootDir, testDir)), Is.False);
    }
    
    [Test]
    public void TestListFolders()
    {
        var rootDir = _serviceProvider.GetRequiredService<IOptions<InAppSettings>>().Value.PageRootDirectory;

        // Prepare some test directories
        var testDirs = new List<string> { "TestFolder1", "TestFolder2" };
        foreach (var testDir in testDirs)
        {
            Directory.CreateDirectory(Path.Combine(rootDir, testDir));
        }

        var controller = _serviceProvider.GetRequiredService<DirectoryController>();

        // Call the method and deserialize the result
        var resultJson = controller.ListFolders();
        var result = JsonConvert.DeserializeObject<List<string>>(resultJson);

        // Assert that the correct folders were returned
        Assert.That(result, Is.EquivalentTo(testDirs));

        // Cleanup
        foreach (var testDir in testDirs)
        {
            Directory.Delete(Path.Combine(rootDir, testDir));
        }
    }
    
    [Test]
    public async Task TestUploadFile_AllowedFileType()
    {
        // Arrange
        // Use Moq or another mocking framework to create a mock IFormFile
        var mockFile = new Mock<IFormFile>();
        // Set up the mock file's properties and behavior
        mockFile.Setup(f => f.FileName).Returns("test.txt");
        mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    
        // Get DirectoryController from services
        var controller = _serviceProvider.GetRequiredService<DirectoryController>();
    
        // Act
        var result = await controller.UploadFile(mockFile.Object, "TestFolder1");

        // Assert
        // Check that the method returned an OkResult
        Assert.IsInstanceOf<OkResult>(result);
    }

    [Test]
    public void TestDeleteFile_FileExists()
    {
        // Arrange
        string rootDir = _serviceProvider.GetRequiredService<IOptions<InAppSettings>>().Value.PageRootDirectory;
        string testFile = "test.txt";
        string folderPath = Path.Combine(rootDir, "TestFolder1", "Uploads");

        // Ensure the directory exists
        Directory.CreateDirectory(folderPath);

        // Now you can safely write the file
        File.WriteAllText(Path.Combine(folderPath, testFile), "Hello, World!");

        // Get DirectoryController from services
        var controller = _serviceProvider.GetRequiredService<DirectoryController>();

        // Act
        var result = controller.DeleteFile("TestFolder1", testFile);

        // Assert
        // Check that the method returned an OkResult
        Assert.IsInstanceOf<OkResult>(result);
        // Check that the file was deleted
        Assert.False(File.Exists(Path.Combine(rootDir, "TestFolder1", "Uploads", testFile)));
    }

    [Test]
    public void TestAllowedFileType_AllowedType()
    {
        // Arrange
        // Get DirectoryController from services
        var controller = _serviceProvider.GetRequiredService<DirectoryController>();

        // Act
        var result = controller.AllowedFileType("test.txt", "TestFolder1");

        // Assert
        // Check that the method returned true
        Assert.True(result);
    }


    [TearDown]
    public void TearDown()
    {
        // Cleanup created directories after each test
        var rootDir = _serviceProvider.GetRequiredService<IOptions<InAppSettings>>().Value.PageRootDirectory;
        if (Directory.Exists(rootDir))
        {
            Directory.Delete(rootDir, recursive: true);
        }
    }
}