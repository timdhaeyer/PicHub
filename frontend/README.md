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

Routing notes:

- The admin console is the landing page of the site (open `/` in your browser).
- Album upload pages are only accessible via a public album link at `/albums/{token}`. The upload UI will not be shown unless a valid token is present in the URL.

Admin auth stub:

- The backend admin endpoints expect a header `X-Admin-Auth` with a token.
- Default dev token: `dev-secret` (for local testing only — DO NOT USE IN PRODUCTION).

To set a token for the frontend client, store it in `localStorage` before using the admin UI:

```javascript
localStorage.setItem('ADMIN_AUTH_TOKEN', 'dev-secret');
```

This will cause the admin create call to include the header automatically.

If your backend is running on a different host/port than the frontend (common during local development), set the `VITE_API_BASE` environment variable before starting Vite. Example (PowerShell):

```powershell
$env:VITE_API_BASE = 'http://localhost:7071'
npm run dev
```

The frontend will use `VITE_API_BASE` to construct API URLs (defaults to same-origin when not set).

Local Azurite usage (PowerShell)

```powershell
# Start Azurite in the frontend folder
npx azurite --silent --location ./azurite --blobPort 10000 --queuePort 10001

# Export the connection string for Azurite (default credentials)
$env:AZURE_STORAGE_CONNECTION_STRING = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqX2...==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1"

# Start the local express proxy that will upload to Azurite when connection string is set
npm run start-server
```
