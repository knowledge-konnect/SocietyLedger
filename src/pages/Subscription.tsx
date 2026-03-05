"use client"

import Navbar from '../components/layout/Navbar'
import { useState } from 'react'
import { CheckCircle, ArrowRight, Zap, Shield, Users, BarChart3, Clock, Mail } from 'lucide-react'
import { useNavigate } from 'react-router-dom'

export default function Subscription() {
  const navigate = useNavigate()
  const [isActivating, setIsActivating] = useState(false)

  const plan = {
    id: 'pro',
    name: 'SocietyLedger Pro',
    price: '₹299',
    period: '/month after trial',
    description: '1 month free, then ₹299/month',
    tagline: 'Full access to all features',
    trial: '1 Month Free • No Credit Card Required',
    features: [
      'Unlimited flats & residents',
      'Create & manage maintenance bills',
      'Record payments (cash, cheque, online transfer)',
      'Advanced charts & reports',
      'Up to 5 admin accounts with roles',
      'Real-time KPI dashboard',
      'Track all expenses & income',
      'Payment history & receipt management',
      'Email support',
    ],
  }

  const handleStartTrial = async () => {
    setIsActivating(true)
    try {
      // Redirect to signup to start free trial
      setTimeout(() => {
        navigate('/signup')
        setIsActivating(false)
      }, 500)
    } catch (error) {
      console.error('Error:', error)
      setIsActivating(false)
    }
  }

  return (
    <div className="min-h-screen bg-background">
      <Navbar variant="landing" />

      {/* Hero Section */}
      <section className="pt-20 md:pt-32 pb-16 md:pb-20 px-4 sm:px-6 lg:px-8 bg-gradient-to-b from-primary/5 to-transparent">
        <div className="max-w-4xl mx-auto text-center space-y-6">
          <div className="inline-block px-4 py-2 bg-indigo-50 dark:bg-indigo-900/30 rounded-full border border-indigo-200 dark:border-indigo-700">
            <p className="text-sm font-semibold text-indigo-600 dark:text-indigo-400">✨ 1 Month Free • No Credit Card Required</p>
          </div>

          <div className="space-y-4">
            <h1 className="text-4xl md:text-5xl font-bold text-slate-900 dark:text-white leading-tight">
              Start Your Free Trial Today
            </h1>
            <p className="text-lg text-slate-700 dark:text-slate-300 max-w-2xl mx-auto">
              Get 1 month completely free. Manage flats, create bills, record payments, and generate reports. No credit card required
            </p>
          </div>
        </div>
      </section>

      {/* Pricing Plan */}
      <section className="py-16 md:py-28 px-4 sm:px-6 lg:px-8">
        <div className="max-w-3xl mx-auto">
          {/* Badge */}
          <div className="text-center mb-6">
            <span className="inline-block px-4 py-2 bg-gradient-to-r from-indigo-600 to-indigo-500 text-white rounded-full text-sm font-semibold shadow-lg">
              ⭐ Most Popular
            </span>
          </div>

          <div className="rounded-2xl border-2 border-indigo-600 dark:border-indigo-500 shadow-xl overflow-hidden bg-white dark:bg-slate-900">
            {/* Content */}
            <div className="p-8 md:p-12 space-y-8">
              {/* Plan Header */}
              <div className="space-y-3">
                <h3 className="text-3xl md:text-4xl font-bold text-slate-900 dark:text-white">{plan.name}</h3>
                <p className="text-base font-medium text-indigo-600 dark:text-indigo-400">{plan.tagline}</p>
              </div>

              {/* Price Section */}
              <div className="space-y-2 border-t border-b border-slate-200 dark:border-slate-800 py-6">
                <div className="flex items-baseline gap-2">
                  <span className="text-6xl font-bold text-slate-900 dark:text-white">{plan.price}</span>
                  <span className="text-lg text-slate-600 dark:text-slate-400">{plan.period}</span>
                </div>
                <p className="text-sm text-indigo-600 dark:text-indigo-400 font-semibold">{plan.trial}</p>
                <p className="text-xs text-slate-600 dark:text-slate-400 pt-1">Then cancel anytime, no lock-in.</p>
              </div>

              {/* Features */}
              <ul className="space-y-3.5">
                {plan.features.map((feature, i) => (
                  <li key={i} className="flex items-start gap-3">
                    <CheckCircle className="w-5 h-5 text-indigo-600 dark:text-indigo-400 flex-shrink-0 mt-0.5" />
                    <span className="text-sm text-slate-700 dark:text-slate-300 font-medium">{feature}</span>
                  </li>
                ))}
              </ul>

              {/* CTA Button */}
              <button
                onClick={handleStartTrial}
                disabled={isActivating}
                className="w-full py-4 px-6 rounded-xl font-semibold transition-all duration-300 flex items-center justify-center gap-2 bg-gradient-to-r from-indigo-600 to-indigo-500 text-white hover:shadow-lg hover:-translate-y-1 shadow-lg disabled:opacity-70 hover:from-indigo-700 hover:to-indigo-600 dark:from-indigo-600 dark:to-indigo-500"
              >
                {isActivating ? (
                  <>
                    <div className="w-4 h-4 border-2 border-current border-t-transparent rounded-full animate-spin" />
                    Redirecting...
                  </>
                ) : (
                  <>
                    Start Your Free Trial
                    <ArrowRight className="w-5 h-5" />
                  </>
                )}
              </button>
            </div>
          </div>

          {/* Trust Indicators */}
          <div className="mt-8 grid grid-cols-3 gap-4 text-center">
            <div>
              <p className="text-2xl font-bold text-slate-900 dark:text-white">1 Month</p>
              <p className="text-xs text-slate-600 dark:text-slate-400 mt-1">Completely Free</p>
            </div>
            <div>
              <p className="text-2xl font-bold text-slate-900 dark:text-white">No Card</p>
              <p className="text-xs text-slate-600 dark:text-slate-400 mt-1">Required</p>
            </div>
            <div>
              <p className="text-2xl font-bold text-slate-900 dark:text-white">Cancel</p>
              <p className="text-xs text-slate-600 dark:text-slate-400 mt-1">Anytime</p>
            </div>
          </div>
        </div>
      </section>

      {/* Features Comparison */}
      <section className="py-16 md:py-24 px-4 sm:px-6 lg:px-8 bg-slate-50 dark:bg-slate-900/50 border-t border-slate-200 dark:border-slate-800">
        <div className="max-w-6xl mx-auto">
          <div className="text-center mb-16 space-y-4">
            <h2 className="text-3xl md:text-4xl font-bold text-foreground">What's included</h2>
            <p className="text-lg text-muted-foreground max-w-2xl mx-auto">
              Everything you need to manage your society effectively
            </p>
          </div>

          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
            {[
              {
                icon: Zap,
                title: 'Unlimited Flats',
                description: 'Add unlimited flats and residents to your society management',
              },
              {
                icon: Shield,
                title: 'Secure Data Storage',
                description: 'Bank-grade encryption with encrypted storage and daily backups',
              },
              {
                icon: Users,
                title: 'Team Collaboration',
                description: 'Up to 5 admin accounts with flexible role-based permissions',
              },
              {
                icon: BarChart3,
                title: 'Advanced Reports',
                description: 'Charts, analytics, and comprehensive financial reports',
              },
              {
                icon: Clock,
                title: 'Payment Recording',
                description: 'Record offline cash, cheque, and online transfer payments',
              },
              {
                icon: Mail,
                title: 'Email Support',
                description: 'Get help via email whenever you need assistance',
              },
            ].map((feature, i) => (
              <div key={i} className="p-6 rounded-xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-800/50 hover:border-indigo-300 dark:hover:border-indigo-600 hover:shadow-md transition-colors">
                <feature.icon className="w-8 h-8 text-indigo-600 dark:text-indigo-400 mb-4" />
                <h3 className="font-semibold text-slate-900 dark:text-white mb-2">{feature.title}</h3>
                <p className="text-sm text-slate-600 dark:text-slate-400">{feature.description}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* How It Works */}
      <section className="py-16 md:py-24 px-4 sm:px-6 lg:px-8">
        <div className="max-w-4xl mx-auto">
          <div className="text-center mb-12 space-y-4">
            <h2 className="text-3xl font-bold text-foreground">How It Works</h2>
            <p className="text-slate-600 dark:text-slate-400">Get your society up and running in minutes</p>
          </div>

          <div className="space-y-8">
            {[
              {
                step: '1',
                title: 'Sign Up',
                description: 'Create your account with your society name, email, and phone number.',
              },
              {
                step: '2',
                title: 'Add Flats & Residents',
                description: 'Enter flat numbers, owner names, emails, and maintenance amounts.',
              },
              {
                step: '3',
                title: 'Generate Bills',
                description: 'Create maintenance bills with customized amounts and due dates.',
              },
              {
                step: '4',
                title: 'Collect Payments',
                description: 'Collect payments from residents offline (cash, cheque, or personal UPI).',
              },
              {
                step: '5',
                title: 'Record & Track',
                description: 'Manually enter payment details in the app and generate reports.',
              },
            ].map((item, i) => (
              <div key={i} className="flex gap-6">
                <div className="flex-shrink-0">
                  <div className="flex items-center justify-center h-12 w-12 rounded-xl bg-indigo-600 dark:bg-indigo-500 text-white font-bold text-lg">
                    {item.step}
                  </div>
                </div>
                <div>
                  <h3 className="font-semibold text-slate-900 dark:text-white mb-1">{item.title}</h3>
                  <p className="text-slate-600 dark:text-slate-400">{item.description}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* FAQ Section */}
      <section className="py-16 md:py-24 px-4 sm:px-6 lg:px-8 bg-slate-50 dark:bg-slate-900/50 border-t border-slate-200 dark:border-slate-800">
        <div className="max-w-4xl mx-auto">
          <div className="text-center mb-12 space-y-4">
            <h2 className="text-3xl font-bold text-foreground">Frequently Asked Questions</h2>
            <p className="text-slate-600 dark:text-slate-400">Have questions? We've got answers</p>
          </div>

          <div className="space-y-4">
            {[
              {
                q: 'How does the free trial work?',
                a: 'You get 1 month completely free with full access to all features. No credit card required to start. After 30 days, it\'s ₹299/month. You can cancel anytime.',
              },
              {
                q: 'Is my data secure?',
                a: 'Yes, we use bank-grade encryption, SSL certificates, and daily automated backups to protect your data.',
              },
              {
                q: 'How are payments collected?',
                a: 'Payments are collected offline by the society admin (cash, cheque, or personal UPI). You manually enter payment details in the app for record-keeping and reporting.',
              },
              {
                q: 'Can I add unlimited flats?',
                a: 'Yes, the plan includes unlimited flats. Add as many flats and residents as your society has.',
              },
              {
                q: 'Can I cancel anytime?',
                a: 'Yes, you can cancel your subscription at any time, even during the free trial. No questions asked, no hidden charges.',
              },
              {
                q: 'How do I get support?',
                a: 'Email support is available at support@societyledger.com. We respond to all support requests within 24 hours.',
              },
            ].map((faq, i) => (
              <div key={i} className="p-6 rounded-xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-800/50 hover:border-indigo-300 dark:hover:border-indigo-600 transition-colors">
                <h3 className="font-semibold text-slate-900 dark:text-white mb-2">{faq.q}</h3>
                <p className="text-sm text-slate-600 dark:text-slate-400">{faq.a}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA Footer */}
      <section className="py-16 px-4 sm:px-6 lg:px-8 bg-gradient-to-br from-indigo-600 to-indigo-700 dark:from-indigo-600 dark:to-indigo-700">
        <div className="max-w-4xl mx-auto text-center space-y-6">
          <h2 className="text-3xl md:text-4xl font-bold text-white">Ready to get started?</h2>
          <p className="text-lg text-white/90 max-w-2xl mx-auto">
            Start your free trial now. Get 1 month completely free to manage your society. No credit card required.
          </p>
          <button
            onClick={handleStartTrial}
            disabled={isActivating}
            className="inline-block px-8 py-4 bg-white dark:bg-slate-900 text-indigo-600 dark:text-indigo-400 rounded-xl font-semibold shadow-lg hover:shadow-2xl hover:-translate-y-1 transition-all duration-300 border border-indigo-200 dark:border-indigo-700 disabled:opacity-70"
          >
            {isActivating ? 'Redirecting...' : 'Start Free Trial Now'}
          </button>
        </div>
      </section>
    </div>
  )
}
