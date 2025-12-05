import { API_BASE } from '../lib/api'

export function renderGallery(container: HTMLElement) {
  container.innerHTML = `
  <main class="center">
    <h1>PicHub - Gallery</h1>
    <div id="albums">
      <p>Loading albums...</p>
    </div>
  </main>
  `

  const albumsEl = document.getElementById('albums')!

  async function loadAlbums() {
    try {
      const res = await fetch(`${API_BASE}/api/albums`)
      if (!res.ok) {
        albumsEl.innerHTML = '<p>Failed to load albums</p>'
        return
      }
      const json = await res.json().catch(() => ([]))
      if (!Array.isArray(json)) {
        albumsEl.innerHTML = '<p>No albums found</p>'
        return
      }
      albumsEl.innerHTML = ''
      json.forEach((a: any) => {
        const d = document.createElement('div')
        d.className = 'album-card'
        const title = document.createElement('h3')
        title.textContent = a.title || a.id || 'Untitled'
        const link = document.createElement('a')
        const token = a.publicToken || a.token || a.id
        link.href = token ? `/albums/${token}` : '#'
        link.textContent = 'Open'
        d.appendChild(title)
        d.appendChild(link)
        albumsEl.appendChild(d)
      })
    } catch (err) {
      console.error(err)
      albumsEl.innerHTML = '<p>Error loading albums</p>'
    }
  }

  loadAlbums()
}
