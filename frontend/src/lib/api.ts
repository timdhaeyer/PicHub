/// <reference types="vite/client" />

// Primary: Vite-provided env (statically injected at build/dev start)
// Fallbacks (runtime) so dev server doesn't need a restart when env files are added:
//  - window.__VITE_API_BASE__ (temp global you can set from console)
//  - localStorage.VITE_API_BASE (persisted per-browser)
const staticBase = (import.meta.env.VITE_API_BASE as string) || ''
const runtimeGlobal = typeof window !== 'undefined' ? (window as any).__VITE_API_BASE__ : undefined
const runtimeLocal = typeof window !== 'undefined' ? window.localStorage.getItem('VITE_API_BASE') : null

// Prefer runtime overrides (global/localStorage) so you can switch target during debugging
export const API_BASE = runtimeGlobal || runtimeLocal || staticBase || ''

if (!API_BASE) {
    // Helpful debug output for developers seeing same-origin requests
    // (e.g. requests going to the Vite server at :5173)
    console.warn('API_BASE is empty — requests will go to the frontend origin. Set VITE_API_BASE or run `window.__VITE_API_BASE__ = "http://localhost:7071"` in the console, or set localStorage.setItem("VITE_API_BASE","http://localhost:7071").')
} else {
    console.debug('Using API_BASE =', API_BASE)
}

export function setApiBaseForRuntime(val: string) {
    if (typeof window !== 'undefined') {
        (window as any).__VITE_API_BASE__ = val
    }
}
