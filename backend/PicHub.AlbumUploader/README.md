# AlbumUploader Azure Functions (C# .NET 10)

This is a scaffold for the AlbumUploader Azure Functions project.

See `../..\docs\LOCAL_SETUP.md` for a consolidated local development quickstart covering Azurite, the Functions host, frontend dev server, and running tests.

Run the Functions host locally with the Functions Core Tools (dotnet-isolated worker):

```powershell
cd backend/AlbumUploader
func start --verbose
```

Local storage can be emulated with Azurite (see the docs linked above).
