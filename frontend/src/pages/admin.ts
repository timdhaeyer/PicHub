import { API_BASE } from '../lib/api'

export function renderAdmin(container: HTMLElement) {

  container.innerHTML = `
    <main class="center">
      <h1>PicHub - Admin</h1>
      <p>Welcome to the admin console. Create an album to get a shareable upload link.</p>
      <div id="admin">
        <input id="title" placeholder="Album title" />
        <input id="description" placeholder="Description (optional)" />
        <label>
          <input id="allow-uploads" type="checkbox" checked /> Allow uploads
        </label>
        <label>
          Max file size (MB, optional): <input id="max-file-size" type="number" min="1" />
        </label>
        <label>
          Album size: 
          <select id="album-size">
            <option value="XS">XS</option>
            <option value="S">S</option>
            <option value="M" selected>M</option>
            <option value="L">L</option>
            <option value="XL">XL</option>
          </select>
        </label>
        <button id="create">Create Album</button>
        <div id="admin-status"></div>
        <div id="links"></div>
      </div>
      <hr />
      <p>If you already have a public album link, open it at <code>/albums/&lt;token&gt;</code>.</p>
    </main>
  `

  const title = document.getElementById('title') as HTMLInputElement
  const description = document.getElementById('description') as HTMLInputElement
  const allowUploadsEl = document.getElementById('allow-uploads') as HTMLInputElement
  const maxFileSizeEl = document.getElementById('max-file-size') as HTMLInputElement
  const albumSizeEl = document.getElementById('album-size') as HTMLSelectElement
  const createBtn = document.getElementById('create') as HTMLButtonElement
  const status = document.getElementById('admin-status')!
  const links = document.getElementById('links')!

  createBtn.addEventListener('click', async () => {
    const t = (title.value || '').trim()
    if (!t) {
      status.textContent = 'Please provide a title.'
      return
    }

    status.textContent = 'Creating album...'
    try {
      const authHeader = window.localStorage.getItem('ADMIN_AUTH_TOKEN') || 'dev-secret'

      const payload: any = {
        title: t,
        description: (description.value || '').trim(),
        allowUploads: !!allowUploadsEl.checked,
      }

      const maxSizeVal = parseInt(maxFileSizeEl.value || '', 10)
      if (!Number.isNaN(maxSizeVal) && maxSizeVal > 0) payload.maxFileSizeMb = maxSizeVal
      const albumSize = (albumSizeEl.value || 'M')
      payload.albumSizeTshirt = albumSize

      const res = await fetch(`${API_BASE}/api/admin/albums`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json', 'X-Admin-Auth': authHeader },
          body: JSON.stringify(payload)
        })

      if (!res.ok) {
        const txt = await res.text()
        status.textContent = `Create failed: ${txt}`
        return
      }

      const json = await res.json()
      const token = json.publicToken || json.token || json.id
      const url = json.publicUrl || (token ? `/albums/${token}` : null)
      status.textContent = 'Album created.'
      if (url) {
        links.innerHTML = `<p>Share link: <a href="${url}">${url}</a></p>`
        // Redirect admin to the public album upload page so they can preview or open the link
        window.location.href = url
      } else {
        links.textContent = JSON.stringify(json)
      }
    } catch (err) {
      console.error(err)
      status.textContent = 'Create failed (network error)'
    }
  })
}
