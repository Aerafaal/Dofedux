using Dofedux.Models;

namespace Dofedux.Services.Downloader;

public interface IDownloaderService
{
    Task<FileInformation> DownloadAsync(string url, CancellationToken cancellationToken = default);
}