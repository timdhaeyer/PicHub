const express = require('express')
const multer = require('multer')
const path = require('path')
const fs = require('fs')

const upload = multer({ dest: path.join(__dirname, 'uploads') })
const app = express()
const PORT = process.env.PORT || 3000

app.use(express.static(path.join(__dirname, '..', 'dist')))
const { AZURE_STORAGE_CONNECTION_STRING } = process.env

let blobClientHelpers = null
if (AZURE_STORAGE_CONNECTION_STRING) {
  try {
    const { BlobServiceClient } = require('@azure/storage-blob')
    const serviceClient = BlobServiceClient.fromConnectionString(AZURE_STORAGE_CONNECTION_STRING)
    blobClientHelpers = { serviceClient }
    console.log('Azure Storage connection detected; uploads will be saved to blob storage')
  } catch (e) {
    console.warn('Could not load @azure/storage-blob, falling back to local uploads:', e.message)
  }
}

app.post('/api/upload', upload.array('files'), async (req, res) => {
  const uploaded = []

  if (blobClientHelpers) {
    // upload each file to Azurite / Azure Storage
    const containerName = 'pichub-local'
    const containerClient = blobClientHelpers.serviceClient.getContainerClient(containerName)
    try {
      await containerClient.createIfNotExists()
    } catch (err) {
      console.error('Error ensuring container exists', err.message)
    }

    for (const f of (req.files || [])) {
      const blobName = `${Date.now()}-${f.originalname.replace(/[^a-zA-Z0-9.\-]/g, '_')}`
      const blockBlobClient = containerClient.getBlockBlobClient(blobName)
      const stream = fs.createReadStream(f.path)
      try {
        await blockBlobClient.uploadStream(stream)
        uploaded.push({ originalname: f.originalname, url: blockBlobClient.url })
      } catch (err) {
        console.error('Upload error', err.message)
      }
    }
    // cleanup local temp files
    for (const f of (req.files || [])) {
      try { fs.unlinkSync(f.path) } catch (e) {}
    }
    return res.json({ files: uploaded })
  }

  // Fallback: Simple local stub - return URLs to uploaded files in the uploads folder
  const files = (req.files || []).map(f => ({ originalname: f.originalname, url: `/local-uploads/${path.basename(f.path)}` }))
  res.json({ files })
})

app.use('/local-uploads', express.static(path.join(__dirname, 'uploads')))

app.listen(PORT, () => console.log(`Local dev server listening on http://localhost:${PORT}`))
