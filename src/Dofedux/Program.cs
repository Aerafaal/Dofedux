
using Dofedux.Services.Downloader;
using Dofedux.Services.Retro;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var services = new ServiceCollection()
    .AddLogging(x => x.ClearProviders().AddSerilog())
    .AddSingleton<IDownloaderService, DownloaderService>()
    .AddSingleton<IRetroService, RetroService>()
    .BuildServiceProvider();

await services.GetRequiredService<IRetroService>().DownloadRetroCdnAsync();

Console.ReadKey();