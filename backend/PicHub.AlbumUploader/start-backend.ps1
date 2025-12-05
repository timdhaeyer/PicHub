<#
Builds the PicHub.AlbumUploader project and starts the Azure Functions host
for local development. Intended to be run from the repository (or directly
from this folder). This script is for Windows/PowerShell.

Usage:
  .\start-backend.ps1

Behavior:
  - Runs `dotnet build` in the project folder
  - If build succeeds, starts `func start --script-root bin\Debug\net9.0`
  - For convenience, sets `ASPNETCORE_ENVIRONMENT=Development` and uses
    `local.settings.json` values.
#>

$projectDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Write-Host "Project directory: $projectDir"

Push-Location $projectDir
try {
    Write-Host "Building project..."
    dotnet build -c Debug
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed. Fix compilation errors before starting the Functions host."
        exit 1
    }

    Write-Host "Starting Functions host (development)..."
    # Use the built output as the script root so the host can find the compiled functions
    func start --script-root "bin\Debug\net9.0"
}
finally {
    Pop-Location
}
