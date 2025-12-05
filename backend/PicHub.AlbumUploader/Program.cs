using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using MediatR;
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

        // Prefer EF-backed repository if available, otherwise fall back to the Dapper-based AlbumRepository
        if (Type.GetType("PicHub.AlbumUploader.Services.EfAlbumRepository, PicHub.AlbumUploader") != null)
        {
            services.AddScoped(typeof(IAlbumRepository), Type.GetType("PicHub.AlbumUploader.Services.EfAlbumRepository, PicHub.AlbumUploader"));
        }
        else
        {
            services.AddScoped<IAlbumRepository, AlbumRepository>();
        }
        services.AddSingleton<AdminAuthService>();
        services.AddSingleton<QuotaService>();
        services.AddSingleton<FileValidationService>();

        var storageConn = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ?? Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        if (!string.IsNullOrEmpty(storageConn))
        {
            services.AddSingleton(new BlobServiceClient(storageConn));
        }

        // Register MediatR handlers (CQRS)
        services.AddMediatR(typeof(Program));
    })
    .Build();

await host.RunAsync();
