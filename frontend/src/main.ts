console.log('Vite environment variables loaded:', import.meta.env);
import './styles.css'
import { init } from './app'

function navigateTo(path: string) {
	if (window.location.pathname === path) return
	history.pushState({}, '', path)
	init()
}

// Intercept local navigation clicks so SPA routes render without full reload
document.addEventListener('click', (ev) => {
	const target = ev.target as Element | null
	if (!target) return
	const anchor = target.closest && target.closest('a') as HTMLAnchorElement | null
	if (!anchor) return
	const href = anchor.getAttribute('href')
	if (!href) return
	// Ignore external links, mailto, download, and anchors with target
	if (href.startsWith('http') || href.startsWith('mailto:') || anchor.target === '_blank') return
	// Only handle same-origin absolute or root-relative paths
	if (href.startsWith('/')) {
		ev.preventDefault()
		navigateTo(href)
	}
})

window.addEventListener('popstate', () => init())

// Initial mount
init()
