import type { Metadata } from 'next'
import { ReactNode } from 'react'
import '../src/index.css'

export const metadata: Metadata = {
  title: 'SocietyLedger - Modern Society Management',
  description: 'Complete society management platform with billing, payments, expenses, and analytics. Simplify apartment administration.',
  icons: {
    icon: '/icon.svg',
    apple: '/apple-icon.png',
  },
    generator: 'v0.app'
}

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body className="font-sans antialiased">
        {children}
      </body>
    </html>
  )
}
