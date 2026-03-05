// Entry point for v0 preview - renders the full Vite React app
import React from 'react'

// Import the complete app with all providers and routing
const App = React.lazy(() => import('../src/App').then(m => ({ default: m.default })))

export default function Page() {
  return (
    <React.Suspense fallback={
      <div className="flex items-center justify-center min-h-screen bg-background">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto mb-4"></div>
          <p className="text-muted-foreground">Loading SocietyLedger...</p>
        </div>
      </div>
    }>
      <App />
    </React.Suspense>
  )
}
