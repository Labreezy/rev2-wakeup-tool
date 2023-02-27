using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using GGXrdReversalTool.Library.Configuration;
using GGXrdReversalTool.Library.GitHub;
using GGXrdReversalTool.Library.Logging;
using GGXrdReversalTool.Library.Versioning;

namespace GGXrdReversalTool.Updates;

public class UpdateManager
{
    private readonly GitHubClient _gitHubClient = new();
    private const string ReleaseFolder = "Release";

    public void CleanOldFiles()
    {
        foreach (var fileName in Directory.GetFiles(Directory.GetCurrentDirectory()))
        {
            FileInfo fi = new FileInfo(fileName);

            if (fi.Name.EndsWith($"_OLD{fi.Extension}"))
            {
                fi.Delete();
            }
        }
        DeleteFolder(ReleaseFolder);
    }

    public ReversalToolVersion CheckUpdates()
    {
        try
        {
            return _gitHubClient.GetLatestVersion();
        }
        catch (Exception)
        {
            LogManager.Instance.WriteLine("Failed to check for updates");
            throw;
        }
    }

    public bool DownloadUpdate(ReversalToolVersion reversalToolVersion)
    {
        string zipFile = $"{ReleaseFolder}_{Guid.NewGuid()}.zip";


        //TODO Replace by HttpClient
        using (WebClient client = new WebClient())
        {
            client.DownloadFile(reversalToolVersion.Url, zipFile);
        }

        DeleteFolder(ReleaseFolder);

        System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, Directory.GetCurrentDirectory());

        File.Delete(zipFile);


        return true;
    }

    public bool InstallUpdate()
    {
        string currentDirectory = Directory.GetCurrentDirectory();

        foreach (var fileName in Directory.GetFiles(currentDirectory))
        {
            var logManagerFileName = LogManager.Instance.FileName;
            if (!fileName.EndsWith(logManagerFileName))
            {
                FileInfo fi = new FileInfo(fileName);

                string newPath = $"{fi.Directory}\\{Path.GetFileNameWithoutExtension(fileName)}_OLD{fi.Extension}";

                File.Move(fileName, newPath);
            }
        }

        foreach (var fileName in Directory.GetFiles($"{currentDirectory}\\{ReleaseFolder}"))
        {
            FileInfo fi = new FileInfo(fileName);
            File.Move(fileName, $"{currentDirectory}\\{fi.Name}");
        }

        DeleteFolder(ReleaseFolder);


        return true;
    }

    public void SaveVersion(ReversalToolVersion version)
    {
        var config = ReversalToolConfiguration.GetConfig();
        config.CurrentVersion = version.Version;
        ReversalToolConfiguration.SaveConfig(config);
    }

    public void RestartApplication()
    {
        var file = $"{System.AppDomain.CurrentDomain.BaseDirectory}/GGXrdReversalTool.exe";
        
        if (!File.Exists(file))
        {
            return;
        }

        Process.Start(file);
        Environment.Exit(1);
    }
    
    private void DeleteFolder(string folder)
    {
        if (Directory.Exists(folder))
        {
            Directory.Delete(folder, true);
        }
    }

}