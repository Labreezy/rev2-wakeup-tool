using System;
using Microsoft.Extensions.Configuration;

namespace GGXrdReversalTool.Configuration;

//TODO Refactor
public static class ReversalToolConfiguration
{
    public static string Get(string key)
    {
        var configuration =  new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");
            
        var config = configuration.Build();

        return config.GetSection(key).Value ?? "";
    }
}