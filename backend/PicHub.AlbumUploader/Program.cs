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
        var efRepoType = Type.GetType("PicHub.AlbumUploader.Services.EfAlbumRepository, PicHub.AlbumUploader");
        if (efRepoType != null)
        {
            services.AddScoped(typeof(IAlbumRepository), efRepoType);
        }
        else
        {
            services.AddScoped<IAlbumRepository, AlbumRepository>();
        }
        services.AddSingleton<AdminAuthService>();
        services.AddSingleton<QuotaService>();
        services.AddSingleton<FileValidationService>();

        var storageConn = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ?? Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        if (string.IsNullOrEmpty(storageConn))
        {
            throw new InvalidOperationException("AZURE_STORAGE_CONNECTION_STRING or AzureWebJobsStorage must be set for blob storage access.");
        }

        var client = new BlobServiceClient(storageConn);
        services.AddSingleton(client);
        services.AddSingleton<PicHub.AlbumUploader.Services.Storage.IBlobService, PicHub.AlbumUploader.Services.Storage.AzureBlobService>();

        // Register MediatR handlers (CQRS)
        services.AddMediatR(typeof(Program));
    })
    .Build();

await host.RunAsync();
