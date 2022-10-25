using Dofedux.Models;

namespace Dofedux.Services.Downloader;

public sealed class DownloaderService : IDownloaderService
{
    private const string DofusRetroCdn = "https://dofusretro.cdn.ankama.com/";
    
    public async Task<FileInformation> DownloadAsync(string url, CancellationToken cancellationToken = default)
    {
        using var httpClient = new HttpClient();
        
        var response = await httpClient.GetAsync(string.Concat(DofusRetroCdn, url), cancellationToken);

        return new FileInformation(url, Path.GetFileName(url), response.Content);
    }
}