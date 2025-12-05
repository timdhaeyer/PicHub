# Quickstart: Run Album Uploader locally

Prerequisites:

- Node.js (16+)
- npm or pnpm
- Azurite (for blob emulation) - `npx azurite` or install globally

1. Start Azurite (PowerShell):

   ```powershell
   npx azurite --silent --location ./azurite --blobPort 10000 --queuePort 10001
   ```

2. In one terminal, start local server (Express proxy) which will upload to Azurite when `AZURE_STORAGE_CONNECTION_STRING` is set:

   ```powershell
   cd frontend
   npm install
   $env:AZURE_STORAGE_CONNECTION_STRING = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlq...;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;"
   npm run start-server
   ```

3. In another terminal, run the Vite frontend:

   ```powershell
   cd frontend
   npm run dev
   ```

4. Open `http://localhost:5173` (or the port printed) on a mobile device or simulator and test upload via the public album flow.

Notes:

- The local server saves files under `frontend/local-dev/uploads` if no AZURE connection string is set.
- To test the full Azure blob flow locally, set `AZURE_STORAGE_CONNECTION_STRING` as above and start Azurite.

\*\*\* End of quickstart
