using Microsoft.Extensions.Configuration;

namespace WebAppTests;

public class AppSettingsTests
{
    private IConfiguration _config;

    [SetUp]
    public void Setup()
    {
        var projectDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(projectDir, "appsettings.json");

        _config = new ConfigurationBuilder()
            .AddJsonFile(configPath)
            .Build();
    }

    [Test]
    public void TestKeyExistsInConfiguration()
    {
        // Arrange
        var keyToCheck = "InAppSettings:PageRootDirectory";

        // Act
        var keyExists = _config.AsEnumerable().Any(item => item.Key == keyToCheck);

        // Assert
        Assert.IsTrue(keyExists, $"Key {keyToCheck} does not exist in the configuration");
    }

}