# Local development setup

This document gathers the common commands to run PicHub locally for development and testing.

## Prerequisites

- .NET 9+ SDK
- Node.js 16+
- npm or pnpm
- Azurite (optional, for blob emulation)
- Azure Functions Core Tools (optional, for Functions local host)

1. Start Azurite (optional - runs blob + queue emulation)

PowerShell:

```powershell
npx azurite --silent --location ./azurite --blobPort 10000 --queuePort 10001
```

Set the Azurite connection string for other processes (example):

```powershell
$env:AZURE_STORAGE_CONNECTION_STRING = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlq...;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;"
```

2. Run the Azure Functions host (backend)

From the workspace root or the `backend` folder:

```powershell
Set-Location -LiteralPath "./backend/PicHub.AlbumUploader"
func start --verbose
```

The Functions host defaults to `http://localhost:7071` for local development.

3. Run the frontend dev server

From the `frontend` folder:

```powershell
cd frontend
npm install
# Optionally set the API base used by the frontend
$env:VITE_API_BASE = 'http://localhost:7071'
npm run dev
```

4. Run the simple local file-server proxy (optional)

The `frontend/local-dev/server.js` is a convenience Express server that saves uploads locally instead of Azurite. Start it with:

```powershell
cd frontend
npm run start-server
```

5. Run unit tests (backend)

From the `backend` folder:

```powershell
Set-Location -LiteralPath "./backend"
dotnet test .\AlbumUploader.Tests\AlbumUploader.Tests.csproj --nologo --verbosity minimal
```

6. End-to-end quickcheck

- Start Azurite (step 1) and Functions host (step 2)
- Start the frontend dev server (step 3) and open the Vite URL (default `http://localhost:5173`)
- Use the public album upload page (`/albums/{token}`) to test uploads. The frontend will request blob URLs from the Functions host and use the blob proxy to stream content.

## Notes and troubleshooting

- If images fail to load in the browser, ensure the frontend is configured to point at the Functions host (`VITE_API_BASE`) instead of the Vite dev server origin.
- For CI and production, replace local Azurite endpoints with real Azure Storage connection strings and remove dev-only tokens and CORS relaxations.
