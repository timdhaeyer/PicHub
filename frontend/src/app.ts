export function init() {
  const app = document.getElementById('app')!
  app.innerHTML = `
    <main class="center">
      <h1>PicHub - Album</h1>
      <div id="album">
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
      const res = await fetch('/api/upload', { method: 'POST', body: form })
      const json = await res.json()
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
