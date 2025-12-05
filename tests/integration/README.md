Integration tests for PicHub AlbumUploader

Prerequisites:

- .NET SDK
- Azurite (for blob storage emulation): `npm install -g azurite` or `npx azurite`

Run Azurite (PowerShell):

```powershell
npx azurite --silent --location .\azurite --blobPort 10000 --queuePort 10001
```

Set environment variables (example):

```powershell
$env:AZURE_STORAGE_CONNECTION_STRING = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlq...;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;"
$env:SqlConnectionString = "Data Source=.:memory:" # or a local sqlite file for persistent tests
```

Run tests:

```powershell
cd tests\integration
dotnet test
```
