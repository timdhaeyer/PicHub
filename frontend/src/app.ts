import { renderAdmin } from './pages/admin'
import { renderUpload } from './pages/upload'

export function init() {
  const app = document.getElementById('app')!

  const path = window.location.pathname || '/'
  if (path.startsWith('/albums/')) {
    const parts = path.split('/')
    const token = parts[2]
    renderUpload(app, token)
  } else {
    renderAdmin(app)
  }
}
