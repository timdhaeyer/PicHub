import { defineConfig } from 'vite'

export default defineConfig({
  root: 'src',
  envDir: '..',
  server: {
    port: 5173,
  },
})
