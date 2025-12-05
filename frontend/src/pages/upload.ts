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

      const json = await res.json().catch(() => ({}))
      status.textContent = 'Upload complete'
      if (Array.isArray(json.files)) {
        json.files.forEach((f: any) => {
          const img = document.createElement('img')
          img.src = f.url
          img.width = 120
          gallery.appendChild(img)
        })
      }
    } catch (err) {
      console.error(err)
      status.textContent = 'Upload failed'
    }
  })
}
