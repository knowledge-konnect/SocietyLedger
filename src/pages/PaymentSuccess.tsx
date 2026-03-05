"use client"

import { useLocation, useNavigate } from 'react-router-dom'
import Navbar from '../components/layout/Navbar'
import { CheckCircle, Download, Home } from 'lucide-react'

export default function PaymentSuccess() {
  const location = useLocation()
  const navigate = useNavigate()
  const { planId, orderId } = location.state || {}

  const planNames: Record<string, string> = {
    basic: 'Basic',
    standard: 'Standard',
    pro: 'Pro',
  }

  return (
    <div className="min-h-screen bg-background">
      <Navbar variant="landing" />

      <div className="py-16 px-4 sm:px-6 lg:px-8">
        <div className="max-w-2xl mx-auto">
          {/* Success Card */}
          <div className="bg-card rounded-2xl border border-border shadow-lg overflow-hidden">
            {/* Header */}
            <div className="bg-gradient-to-br from-green-500/10 to-emerald-500/10 border-b border-border p-8 text-center">
              <div className="w-20 h-20 bg-gradient-to-br from-green-500 to-emerald-500 rounded-full flex items-center justify-center mx-auto mb-4 shadow-lg">
                <CheckCircle className="w-10 h-10 text-white" />
              </div>
              <h1 className="text-4xl font-bold text-foreground mb-2">Payment Successful!</h1>
              <p className="text-lg text-muted-foreground">Your subscription is now active</p>
            </div>

            {/* Content */}
            <div className="p-8 space-y-8">
              {/* Order Details */}
              <div className="bg-background rounded-xl border border-border p-6 space-y-4">
                <h3 className="font-semibold text-foreground text-lg">Order Details</h3>

                <div className="space-y-3">
                  <div className="flex justify-between items-center pb-3 border-b border-border/30">
                    <span className="text-muted-foreground">Plan</span>
                    <span className="font-semibold text-foreground">{planNames[planId] || 'Standard'} Plan</span>
                  </div>

                  <div className="flex justify-between items-center pb-3 border-b border-border/30">
                    <span className="text-muted-foreground">Order ID</span>
                    <span className="font-mono text-sm text-foreground">{orderId || 'Processing...'}</span>
                  </div>

                  <div className="flex justify-between items-center pb-3 border-b border-border/30">
                    <span className="text-muted-foreground">Status</span>
                    <span className="px-3 py-1 bg-green-500/10 text-green-600 rounded-full text-sm font-semibold">
                      Confirmed
                    </span>
                  </div>

                  <div className="flex justify-between items-center">
                    <span className="text-muted-foreground">Billing Cycle</span>
                    <span className="font-semibold text-foreground">Monthly, auto-renews</span>
                  </div>
                </div>
              </div>

              {/* What's Next */}
              <div className="bg-background rounded-xl border border-border p-6 space-y-4">
                <h3 className="font-semibold text-foreground text-lg">What's Next?</h3>

                <div className="space-y-3">
                  {[
                    { num: 1, title: 'Confirmation Email', desc: 'Check your inbox for the receipt and activation details' },
                    { num: 2, title: 'Access Dashboard', desc: 'Log in to your dashboard to start managing your society' },
                    { num: 3, title: 'Invite Team Members', desc: 'Add committee members and assign permissions' },
                    { num: 4, title: 'Setup Billing', desc: 'Configure maintenance charges and billing preferences' },
                  ].map((step) => (
                    <div key={step.num} className="flex gap-4">
                      <div className="w-8 h-8 bg-primary/20 text-primary rounded-full flex items-center justify-center flex-shrink-0 font-semibold text-sm">
                        {step.num}
                      </div>
                      <div>
                        <p className="font-semibold text-foreground">{step.title}</p>
                        <p className="text-sm text-muted-foreground">{step.desc}</p>
                      </div>
                    </div>
                  ))}
                </div>
              </div>

              {/* Features Unlocked */}
              <div className="bg-gradient-to-br from-primary/5 to-secondary/5 rounded-xl border border-primary/20 p-6 space-y-4">
                <h3 className="font-semibold text-foreground text-lg">Features Unlocked</h3>
                <p className="text-sm text-muted-foreground">You now have access to all {planNames[planId] || 'Standard'} plan features:</p>

                <ul className="space-y-2 text-sm">
                  {planId === 'basic' && [
                    'Up to 50 flats management',
                    'Basic monthly billing',
                    'Payment tracking',
                    '1 admin account',
                  ].map((feat, i) => (
                    <li key={i} className="flex items-center gap-2">
                      <div className="w-1.5 h-1.5 bg-primary rounded-full" />
                      <span className="text-muted-foreground">{feat}</span>
                    </li>
                  ))}

                  {planId === 'standard' && [
                    'Up to 200 flats management',
                    'Advanced billing with penalties & addons',
                    'Unlimited email reminders',
                    'PDF & CSV exports',
                    '2-3 admin accounts',
                    'Priority email support',
                  ].map((feat, i) => (
                    <li key={i} className="flex items-center gap-2">
                      <div className="w-1.5 h-1.5 bg-primary rounded-full" />
                      <span className="text-muted-foreground">{feat}</span>
                    </li>
                  ))}

                  {planId === 'pro' && [
                    'Unlimited flats management',
                    'Advanced billing & analytics',
                    'Unlimited email reminders',
                    'Printable PDF notices',
                    'Advanced charts & insights',
                    'Up to 5 admin accounts',
                    'Faster support response',
                  ].map((feat, i) => (
                    <li key={i} className="flex items-center gap-2">
                      <div className="w-1.5 h-1.5 bg-primary rounded-full" />
                      <span className="text-muted-foreground">{feat}</span>
                    </li>
                  ))}
                </ul>
              </div>

              {/* CTA Buttons */}
              <div className="grid grid-cols-2 gap-4">
                <button
                  onClick={() => navigate('/dashboard')}
                  className="py-3 px-4 bg-gradient-to-r from-primary to-secondary text-white rounded-lg font-semibold hover:shadow-lg transition-all duration-300 flex items-center justify-center gap-2"
                >
                  <Home className="w-4 h-4" />
                  Go to Dashboard
                </button>
                <button
                  className="py-3 px-4 border-2 border-primary text-primary rounded-lg font-semibold hover:bg-primary/5 transition-all duration-300 flex items-center justify-center gap-2"
                >
                  <Download className="w-4 h-4" />
                  Download Invoice
                </button>
              </div>

              {/* Support */}
              <div className="p-4 bg-background rounded-lg border border-border/50 text-center">
                <p className="text-sm text-muted-foreground">
                  Need help? Contact our support team at{' '}
                  <a href="mailto:support@societyledger.com" className="text-primary hover:underline font-semibold">
                    support@societyledger.com
                  </a>
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
