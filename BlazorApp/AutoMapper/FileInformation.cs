using AutoMapper;

namespace BlazorApp.AutoMapper;

public class FileInformationProfile : Profile
{
    public FileInformationProfile()
    {
        CreateMap<dynamic, FileInformation>();
    }
}

public class FileInformation
{
    public FileInformation(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public long Size { get; set; }
}
