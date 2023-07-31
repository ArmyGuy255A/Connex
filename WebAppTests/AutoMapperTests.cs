using System.Dynamic;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using WebApp.AutoMapper;

namespace WebAppTests;

public class AutoMapperTests
{
    private readonly IMapper _mapper;
    private readonly ServiceProvider _serviceProvider;

    public AutoMapperTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        _serviceProvider = serviceCollection.BuildServiceProvider();
        _mapper = _serviceProvider.GetRequiredService<IMapper>();
    }

    [Test]
    public void AutoMapper_Configuration_IsValid()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Test]
    public void AutoMapper_CanMapDynamicToFileInformation()
    {
        dynamic source = new ExpandoObject();
        source.Name = "file1.txt";
        source.Size = 13;

        var destination = _mapper.Map<FileInformation>(source);

        Assert.AreEqual(source.Name, destination.Name);
        Assert.AreEqual(source.Size, destination.Size);
    }
}