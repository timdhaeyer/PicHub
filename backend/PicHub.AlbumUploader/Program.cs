using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using PicHub.AlbumUploader.Data;
using PicHub.AlbumUploader.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        var conn = Environment.GetEnvironmentVariable("SqlConnectionString");
        if (!string.IsNullOrEmpty(conn))
        {
            services.AddDbContext<PicHubDbContext>(opt => opt.UseSqlServer(conn));
        }

        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddSingleton<QuotaService>();
        services.AddSingleton<FileValidationService>();

        var storageConn = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ?? Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        if (!string.IsNullOrEmpty(storageConn))
        {
            services.AddSingleton(new BlobServiceClient(storageConn));
        }
    })
    .Build();

await host.RunAsync();
