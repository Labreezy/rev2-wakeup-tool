using Microsoft.Extensions.Configuration;

namespace GGXrdReversalTool.Library.Configuration;

//TODO Refactor
public static class ReversalToolConfiguration
{
    //TODO Make generic
    public static string Get(string key)
    {
        var configuration =  new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");
            
        var config = configuration.Build();

        return config.GetSection(key).Value ?? "";
    }

    public static void Set(string key, object value)
    {
        //TODO Implement
        
        
        // var configuration =  new ConfigurationBuilder()
        //     .AddJsonFile($"appsettings.json");
        //     
        // var config = configuration.Build();

        // configuration.Properties.
        
        
    }
}