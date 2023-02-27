using System.Net;
using System.Text.Json;
using GGXrdReversalTool.Library.Logging;
using GGXrdReversalTool.Library.Versioning;

namespace GGXrdReversalTool.Library.GitHub;

public class GitHubClient
{
    public ReversalToolVersion GetLatestVersion()
    {
        try
        {
            var url = "https://api.github.com/repos/Iquis/rev2-wakeup-tool/releases/latest";

            //TODO refactor?
            using var webClient = new WebClient();
            var dummyUserAgent = "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            webClient.Headers["User-Agent"] = dummyUserAgent;
            string stringResult = webClient.DownloadString(url);

            var responseObject = JsonSerializer.Deserialize<GitHubRelease>(stringResult);

            if (responseObject == null)
            {
                throw new Exception("Cannot deserialize Github Response");
            }

            var downloadUrl = responseObject.Assets.FirstOrDefault()?.DownloadUrl;


            if (downloadUrl == null)
            {
                throw new Exception("Cannot find release download url");
            }
            
            
            return new ReversalToolVersion()
            {
                Version = new Version(responseObject.TagName),
                Url = downloadUrl
            };
        }
        catch (Exception e)
        {
            LogManager.Instance.WriteLine("Failed to get LatestVersion");
            LogManager.Instance.WriteException(e);
            throw;
        }
    }
}
