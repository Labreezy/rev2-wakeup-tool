using System.Text.Json.Serialization;

namespace GGXrdReversalTool.Library.GitHub;

public class GitHubRelease
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; }

    [JsonPropertyName("author")]
    public Author Author { get; set; }
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
        
    [JsonPropertyName("body")]
    public string Body { get; set; }
    
    [JsonPropertyName("assets")]
    public List<Asset> Assets { get; set; }
}

public class Author
{
    [JsonPropertyName("login")]
    public string login { get; set; }
        
    [JsonPropertyName("url")]
    public string url { get; set; }
}

public class Asset
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("browser_download_url")]
    public string DownloadUrl { get; set; }
}


    


