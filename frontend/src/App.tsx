import { useEffect, useState } from 'react'
import './App.css'

type BackendStatus =
  | { state: 'checking'; message: string }
  | { state: 'connected'; message: string }
  | { state: 'error'; message: string }

const apiBaseUrl =
  import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5080'

function App() {
  const [backendStatus, setBackendStatus] = useState<BackendStatus>({
    state: 'checking',
    message: 'Checking backend connection…',
  })

  useEffect(() => {
    const controller = new AbortController()

    async function checkBackend() {
      try {
        const response = await fetch(`${apiBaseUrl}/api/health`, {
          signal: controller.signal,
        })

        if (!response.ok) {
          throw new Error(`Health check returned ${response.status}`)
        }

        const message = await response.text()
        setBackendStatus({ state: 'connected', message })
      } catch (error) {
        if (error instanceof DOMException && error.name === 'AbortError') {
          return
        }

        setBackendStatus({
          state: 'error',
          message:
            error instanceof Error
              ? error.message
              : 'Unable to reach the backend',
        })
      }
    }

    void checkBackend()

    return () => controller.abort()
  }, [])

  return (
    <main className="app-shell">
      <p className="eyebrow">ArgosHound</p>
      <h1>Opportunity intelligence for builders</h1>
      <p className="intro">
        Scout public conversations for product and builder opportunities.
      </p>

      <section className="connection-card" aria-live="polite">
        <span
          className={`status-dot status-dot--${backendStatus.state}`}
          aria-hidden="true"
        />
        <div>
          <h2>Backend connection</h2>
          <p>
            {backendStatus.state === 'connected'
              ? `Connected: ${backendStatus.message}`
              : backendStatus.message}
          </p>
          <code>{apiBaseUrl}/api/health</code>
        </div>
      </section>
    </main>
  )
}

export default App
