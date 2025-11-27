# PicHub Frontend (Vite + TypeScript)

Local dev instructions (uses Azurite for blob storage emulation):

1. Install dependencies

```powershell
cd frontend
npm install
```

2. Run Azurite (optional - the local dev proxy stores files locally)

```powershell
npx azurite --silent --location ./azurite --blobPort 10000 --queuePort 10001
```

3. Start Vite dev server

```powershell
npm run dev
```

4. For the local dev proxy (simple file stub)

```powershell
npm run start-server
```

Notes:

- The `local-dev/server.js` is a simple Express stub saving uploads to `frontend/local-dev/uploads` and serving them at `/local-uploads`.
- For full backend integration, configure the frontend to call your Azure Functions endpoints.

Local Azurite usage (PowerShell)

```powershell
# Start Azurite in the frontend folder
npx azurite --silent --location ./azurite --blobPort 10000 --queuePort 10001

# Export the connection string for Azurite (default credentials)
$env:AZURE_STORAGE_CONNECTION_STRING = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqX2...==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1"

# Start the local express proxy that will upload to Azurite when connection string is set
npm run start-server
```
