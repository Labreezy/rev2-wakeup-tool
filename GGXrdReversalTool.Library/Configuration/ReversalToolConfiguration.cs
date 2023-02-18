using System.Text.Json;
using System.Text.Json.Serialization;
using GGXrdReversalTool.Library.Exceptions;
using Microsoft.Extensions.Configuration;

namespace GGXrdReversalTool.Library.Configuration;

public static class ReversalToolConfiguration
{
    public static ReversalToolConfigObject GetConfig()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build()
            .Get<ReversalToolConfigObject>();


        if (config == null)
        {
            throw new InvalidConfigurationException();
        }

        return config;
    }

    public static void SaveConfig(ReversalToolConfigObject configObject)
    {
        var jsonWriteOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        jsonWriteOptions.Converters.Add(new JsonStringEnumConverter());

        var newJson = JsonSerializer.Serialize(configObject, jsonWriteOptions);
        
        var appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        File.WriteAllText(appSettingsPath, newJson);
    }
    
    public static string Get(string key)
    {
        var configuration =  new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");
            
        var config = configuration.Build();

        return config.GetSection(key).Value ?? "";
    }

}