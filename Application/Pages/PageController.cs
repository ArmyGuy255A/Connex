﻿using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Application.Pages;

[ApiController]
[Route("api/[controller]")]
public class PageController
{
    private readonly string _rootDir;
    private readonly IOptionsMonitor<AppSettings> _settings;

    public PageController(string rootDir, IOptionsMonitor<AppSettings> settings)
    {
        _rootDir = rootDir;
        _settings = settings;
    }
    
    

}