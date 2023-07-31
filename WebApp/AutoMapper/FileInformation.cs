using AutoMapper;

namespace WebApp.AutoMapper;

public class FileInformationProfile : Profile
{
    public FileInformationProfile()
    {
        CreateMap<dynamic, FileInformation>();
    }
}

public class FileInformation
{
    public string Name { get; set; }
    public long Size { get; set; }
}
