using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace PicHub.AlbumUploader.Services;

/// <summary>
/// Service for validating admin API authentication via X-Admin-Auth header.
/// </summary>
public class AdminAuthService
{
    private readonly string _token;
    private readonly ILogger<AdminAuthService>? _logger;
    private readonly bool _isProduction;

    public AdminAuthService(ILogger<AdminAuthService>? logger = null)
    {
        _logger = logger;
        _isProduction = IsProductionEnvironment();

        _token = Environment.GetEnvironmentVariable("ADMIN_AUTH_TOKEN") ?? string.Empty;

        // Validate token configuration
        if (string.IsNullOrWhiteSpace(_token))
        {
            if (_isProduction)
            {
                throw new InvalidOperationException(
                    "ADMIN_AUTH_TOKEN environment variable is required in production");
            }

            // In development, we do NOT hardcode a token here. Instead, developers should set
            // `ADMIN_AUTH_TOKEN` in their `local.settings.json` (Functions) or environment.
            // Example local.settings.json entry (local development only):
            // {
            //   "IsEncrypted": false,
            //   "Values": {
            //     "AzureWebJobsStorage": "UseDevelopmentStorage=true",
            //     "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
            //     "ADMIN_AUTH_TOKEN": "dev-secret"
            //   }
            // }
            _logger?.LogWarning(
                "ADMIN_AUTH_TOKEN not set. For local development, add ADMIN_AUTH_TOKEN to local.settings.json (e.g. 'dev-secret').");
        }
        else if (_token.Length < 32)
        {
            _logger?.LogWarning(
                "ADMIN_AUTH_TOKEN is too short ({Length} chars). Recommend at least 32 characters for security.",
                _token.Length);
        }
    }

    /// <summary>
    /// Validates the admin authentication token from the X-Admin-Auth header.
    /// </summary>
    public bool IsAuthorized(HttpRequestData req)
    {
        if (!req.Headers.TryGetValues("X-Admin-Auth", out var vals))
        {
            _logger?.LogWarning("Admin auth attempt with missing X-Admin-Auth header from {Host}",
                req.Url.Host);
            return false;
        }

        var providedToken = vals.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(providedToken))
        {
            _logger?.LogWarning("Admin auth attempt with empty X-Admin-Auth header from {Host}",
                req.Url.Host);
            return false;
        }

        // Use constant-time comparison to prevent timing attacks
        var isValid = ConstantTimeEquals(providedToken, _token);

        if (!isValid)
        {
            _logger?.LogWarning(
                "Admin auth failed: invalid token from {Host} for {Path}",
                req.Url.Host,
                req.Url.PathAndQuery);
        }

        return isValid;
    }

    /// <summary>
    /// Performs a constant-time string comparison to prevent timing attacks.
    /// </summary>
    private static bool ConstantTimeEquals(string a, string b)
    {
        if (a.Length != b.Length)
        {
            return false;
        }

        var result = 0;
        for (var i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }

        return result == 0;
    }

    /// <summary>
    /// Checks if running in a production environment.
    /// </summary>
    private static bool IsProductionEnvironment()
    {
        var env = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT")
                  ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? "Development";

        return env.Equals("Production", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Generates a random token for development environments.
    /// </summary>
    private static string GenerateDevToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }
}
