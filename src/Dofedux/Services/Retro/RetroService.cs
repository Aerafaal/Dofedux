using Dofedux.Services.Downloader;
using Microsoft.Extensions.Logging;

namespace Dofedux.Services.Retro;

public sealed class RetroService : IRetroService
{
    private static readonly string[] Languages =
    {
        "de", "en", "es", "fr", "it", "nl", "pt"
    };

    private const string BasePath = "lang/swf";
    
    private readonly IDownloaderService _downloader;
    private readonly ILogger<RetroService> _logger;
    
    public RetroService(IDownloaderService downloader, ILogger<RetroService> logger)
    {
        _downloader = downloader;
        _logger = logger;
    }

    public async Task DownloadRetroCdnAsync()
    {
        if (Directory.Exists(BasePath))
            Directory.Delete(BasePath, true);

        Directory.CreateDirectory(BasePath);
        
        await DownloadSwfVersion();
        await DownloadTextVersions();
        
        _logger.LogInformation("Press any key to exit...");
    }

    private async Task DownloadSwfVersion()
    {
        const string path = "lang/versions.swf";
        
        var file = await _downloader.DownloadAsync(path);

        await using var httpStream = await file.Content.ReadAsStreamAsync();

        await using var fileStream = File.OpenWrite(path);

        await httpStream.CopyToAsync(fileStream);
    }

    private async Task DownloadTextVersions()
    {
        foreach (var language in Languages)
        {
            var path = $"lang/versions_{language}.txt";
            
            var file = await _downloader.DownloadAsync(path);
            
            var content = await file.Content.ReadAsStringAsync();
            
            await File.WriteAllTextAsync(path, content);

            var fileNames = from text in content[3..].Split('|')
                where !string.IsNullOrWhiteSpace(text)
                select text.Split(',')
                into split
                select $"{split[0]}_{language}_{split[2]}.swf";

            await DownloadSwfFilesAsync(fileNames);
        }
    }
    
    private Task DownloadSwfFilesAsync(IEnumerable<string> fileNames) =>
        Parallel.ForEachAsync(fileNames, async (fileName, cancellationToken) =>
        {
            _logger.LogInformation("Downloading {FileName}", fileName);
            
            var path = $"{BasePath}/{fileName}";
            
            var file = await _downloader.DownloadAsync(path, cancellationToken);

            await using var httpStream = await file.Content.ReadAsStreamAsync(cancellationToken);

            await using var fileStream = File.OpenWrite(path);

            await httpStream.CopyToAsync(fileStream, cancellationToken);
        });
    
}