using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebApp.AutoMapper;
using WebApp.Classes;
using WebApp.Controllers;

namespace WebAppTests;

public class FolderControllerTests
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
        serviceCollection.AddTransient<FolderController>();

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
        var controller = _serviceProvider.GetRequiredService<FolderController>();
        var result = controller.CreateFolder("TestFolder");

        var rootDir = _serviceProvider.GetRequiredService<IOptions<InAppSettings>>().Value.PageRootDirectory;
        Assert.IsTrue(Directory.Exists(Path.Combine(rootDir, "TestFolder")));
        Assert.IsInstanceOf<OkResult>(result);
    }

    [Test]
    public void TestCreateFolderWithChildren()
    {
        var controller = _serviceProvider.GetRequiredService<FolderController>();
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

        var controller = _serviceProvider.GetRequiredService<FolderController>();

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

    [TearDown]
    public void TearDown()
    {
        // Cleanup created directories after each test
        var rootDir = _serviceProvider.GetRequiredService<IOptions<InAppSettings>>().Value.PageRootDirectory;
        Directory.Delete(rootDir, recursive: true);
    }
}