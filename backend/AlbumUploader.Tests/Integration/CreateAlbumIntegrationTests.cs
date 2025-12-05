using System;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using PicHub.AlbumUploader.Data;
using PicHub.AlbumUploader.Models;
using PicHub.AlbumUploader.Services.Cqrs.Commands;
using PicHub.AlbumUploader.Services.Cqrs.Queries;
using PicHub.AlbumUploader.Services;

namespace PicHub.AlbumUploader.Tests.Integration;

public class CreateAlbumIntegrationTests
{
    [Fact]
    public void CreateAlbum_Then_GetByPublicToken_Works()
    {
        var services = new ServiceCollection();
        services.AddDbContext<PicHubDbContext>(opts => opts.UseInMemoryDatabase("test-db-1"));
        services.AddScoped<IAlbumRepository, PicHub.AlbumUploader.Services.AlbumRepository>();

        var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PicHubDbContext>();

        var repo = scope.ServiceProvider.GetRequiredService<IAlbumRepository>();

        // Simulate CreateAlbumHandler behavior
        var cmd = new CreateAlbumCommand("Integration Test", "desc", true, 50, "M", 7);
        var handler = new PicHub.AlbumUploader.Services.Cqrs.Commands.CreateAlbumHandler(repo);
        var result = handler.Handle(cmd, default).GetAwaiter().GetResult();

        Assert.False(string.IsNullOrEmpty(result.PublicToken));

        // Query using repository
        var album = repo.GetByPublicToken(result.PublicToken);
        Assert.NotNull(album);
        Assert.Equal("Integration Test", album!.Title);
        Assert.Equal(50, album.MaxFileSizeMb);
    }
}
