import { API_BASE } from '../lib/api'

export function renderUpload(container: HTMLElement, token: string) {

  container.innerHTML = `
    <main class="center">
      <h1>PicHub - Album Upload</h1>
      <div id="album">
        <p>Album token: <code id="token">${token}</code></p>
        <input id="file" type="file" multiple accept="image/*,video/*" />
        <button id="upload">Upload</button>
        <div id="status"></div>
        <div id="gallery"></div>
      </div>
    </main>
  `

  const fileInput = document.getElementById('file') as HTMLInputElement
  const uploadBtn = document.getElementById('upload') as HTMLButtonElement
  const status = document.getElementById('status')!
  const gallery = document.getElementById('gallery')!

  uploadBtn.addEventListener('click', async () => {
    const files = fileInput.files
    if (!files || files.length === 0) return
    status.textContent = 'Uploading...'
    const form = new FormData()
    for (let i = 0; i < files.length; i++) {
      form.append('files', files[i])
    }

    try {
      const res = await fetch(`${API_BASE}/api/albums/${encodeURIComponent(token)}/media`, { method: 'POST', body: form })
      if (!res.ok) {
        const txt = await res.text()
        status.textContent = `Upload failed: ${txt}`
        return
      }

      status.textContent = 'Upload complete'
      // Refresh album items
      await loadAlbum()
    } catch (err) {
      console.error(err)
      status.textContent = 'Upload failed'
    }
  })

  async function loadAlbum() {
    try {
      const res = await fetch(`${API_BASE}/api/albums/${encodeURIComponent(token)}`)
      if (!res.ok) return
      const json = await res.json().catch(() => ({}))
      gallery.innerHTML = ''
      if (Array.isArray(json.items)) {
        json.items.forEach((i: any) => {
          // Prefer blobUri if available
          if (i.blobUri) {
            try {
              let url = i.blobUri
              // If blobUri is a relative path (starts with /api/blobs) and API_BASE is set,
              // prefix it so requests go to the backend host instead of the frontend origin.
              if (typeof url === 'string' && url.startsWith('/api/') && API_BASE) {
                url = API_BASE.replace(/\/$/, '') + url
              }
              const ct = i.contentType || ''
              if (ct.startsWith('image/')) {
                const img = document.createElement('img')
                console.debug('Resolved blob URL for image:', url)
                img.src = url
                img.width = 160
                gallery.appendChild(img)
                return
              }
              if (ct.startsWith('video/')) {
                const vid = document.createElement('video')
                vid.src = url
                vid.width = 320
                vid.controls = true
                gallery.appendChild(vid)
                return
              }
              // Fallback: show link
              const a = document.createElement('a')
              a.href = url
              a.textContent = `${i.filename} (${i.size} bytes)`
              a.target = '_blank'
              gallery.appendChild(a)
              return
            } catch (err) {
              console.error('Failed to render blobUri', err)
            }
          }

          // Fallback to filename-only entry
          const p = document.createElement('p')
          p.textContent = `${i.filename} (${i.size} bytes)`
          gallery.appendChild(p)
        })
      }
    } catch (err) {
      console.error(err)
    }
  }

  // Initial load
  loadAlbum()
}
