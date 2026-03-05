"use client"

import { useState, useEffect } from "react"
import { ArrowRight, Building2, DollarSign, BarChart3, Users, Lock, Zap, ChevronRight, Star } from "lucide-react"
import { Link } from 'react-router-dom'
import Navbar from "../components/layout/Navbar"

/**
 * Landing Page - Redesigned with Alytics aesthetic
 * Bold headings, generous whitespace, feature cards with icons, pricing tiers
 */

export default function LandingPageRedesigned() {
  const [activeTestimonial, setActiveTestimonial] = useState(0)
  interface Plan {
    id: string;
    name: string;
    monthlyAmount: number;
    currency: string;
    isActive: boolean;
  }

  const [plans, setPlans] = useState<Plan[]>([])
  const [plansLoading, setPlansLoading] = useState(true)
  const [plansError, setPlansError] = useState<string | null>(null)

  // Static features for the plan
  const planFeatures = [
    { color: "from-blue-500 to-blue-600", icon: Building2, title: "Unlimited flats & residents", description: "Manage unlimited properties and residents" },
    { color: "from-green-500 to-green-600", icon: DollarSign, title: "Create & manage maintenance bills", description: "Easy bill creation and management" },
    { color: "from-purple-500 to-purple-600", icon: BarChart3, title: "Record payments", description: "Cash, cheque, online transfer support" },
    { color: "from-orange-500 to-orange-600", icon: BarChart3, title: "Advanced charts & reports", description: "Detailed analytics and reporting" },
    { color: "from-pink-500 to-pink-600", icon: Users, title: "Up to 5 admin accounts", description: "Role-based access control" },
    { color: "from-indigo-500 to-indigo-600", icon: Zap, title: "Real-time KPI dashboard", description: "Live performance metrics" },
    { color: "from-cyan-500 to-cyan-600", icon: BarChart3, title: "Track all expenses & income", description: "Complete financial tracking" },
    { color: "from-red-500 to-red-600", icon: Lock, title: "Payment history & receipts", description: "Secure transaction records" },
    { color: "from-yellow-500 to-yellow-600", icon: Zap, title: "Email support", description: "24/7 customer support" },
  ]

  // Fetch plans from API
  useEffect(() => {
    const fetchPlans = async () => {
      try {
        setPlansLoading(true)
        // Replace with your actual API endpoint
        const response = await fetch('/api/plans')
        const result = await response.json()
        
        if (result.succeeded && result.data) {
          setPlans(result.data)
        } else {
          setPlansError('Failed to load plans')
        }
      } catch (error) {
        console.error('Error fetching plans:', error)
        setPlansError('Failed to load plans')
      } finally {
        setPlansLoading(false)
      }
    }

    fetchPlans()
  }, [])

  // Transform API data to component format
  const pricingPlans = plans.map(plan => ({
    id: plan.id,
    name: plan.name,
    price: `₹${plan.monthlyAmount}`,
    description: "Everything included. 1 month free, then pay monthly.",
    tagline: "Full access to all features",
    features: planFeatures, // Keep features static
    cta: "Start Free Trial",
    highlighted: true,
    trialText: "1 Month Free • No Credit Card Required",
    monthlyAmount: plan.monthlyAmount,
    currency: plan.currency,
    isActive: plan.isActive
  }))

  const testimonials = [
    {
      name: "Rajesh Kumar",
      role: "Treasurer",
      society: "Sunheart Apartments, Mumbai",
      text: "We started using SocietyLedger last year. Collection is now easy. Members pay on time, and managing finances takes just 2 hours a month.",
      image: "👨‍💼",
    },
    {
      name: "Priya Sharma",
      role: "Secretary",
      society: "Green Valley Society, Bangalore",
      text: "Switched from Excel. Best decision ever. The app is simple, members like it, and office work is much less now.",
      image: "👩‍💼",
    },
    {
      name: "Amit Patel",
      role: "Finance Head",
      society: "Westbrook Towers, Pune",
      text: "No more chasing people for payments. Automatic reminders work great. Now I have time for other society work.",
      image: "👨‍🔧",
    },
  ]

  return (
    <div className="min-h-screen bg-white dark:bg-slate-950">
      <Navbar variant="landing" />

      {/* Hero Section */}
      <section className="pt-32 md:pt-48 pb-24 md:pb-40 px-4 sm:px-6 lg:px-8 relative overflow-hidden bg-gradient-to-br from-white to-slate-50 dark:from-slate-950 dark:to-slate-900">
        <div className="absolute top-0 right-0 w-96 h-96 bg-indigo-100/20 dark:bg-indigo-900/10 rounded-full blur-3xl pointer-events-none" />
        <div className="absolute bottom-0 left-1/4 w-80 h-80 bg-violet-100/20 dark:bg-violet-900/10 rounded-full blur-3xl pointer-events-none" />
        <div className="max-w-6xl mx-auto relative z-10">
          <div className="space-y-8 text-center">
            <div className="inline-block px-4 py-2 bg-indigo-50 dark:bg-indigo-900/30 rounded-full border border-indigo-200 dark:border-indigo-700 animate-fade-in">
              <p className="text-sm font-semibold text-indigo-600 dark:text-indigo-400">✨ Trusted by 500+ societies across India</p>
            </div>

            <h1 className="text-5xl md:text-6xl lg:text-7xl font-bold text-slate-900 dark:text-white text-balance animate-slide-in-up leading-tight">
              Manage Your Society
              <br />
              <span className="bg-gradient-to-r from-indigo-600 to-indigo-500 bg-clip-text text-transparent">Like a Pro</span>
            </h1>

            <p className="text-lg md:text-xl text-slate-700 dark:text-slate-200 max-w-2xl mx-auto leading-relaxed animate-slide-in-up font-medium" style={{ animationDelay: '0.1s' }}>
              Stop using spreadsheets. Manage flats, bills, payments, and generate reports easily. All data in one secure place.
            </p>

            <div className="grid grid-cols-3 gap-4 max-w-2xl mx-auto pt-6 animate-slide-in-up" style={{ animationDelay: '0.15s' }}>
              <div className="text-center">
                <p className="text-2xl font-bold text-indigo-600 dark:text-indigo-400">500+</p>
                <p className="text-sm text-slate-600 dark:text-slate-400">Societies</p>
              </div>
              <div className="text-center">
                <p className="text-2xl font-bold text-indigo-600 dark:text-indigo-400">₹10 Cr+</p>
                <p className="text-sm text-slate-600 dark:text-slate-400">Collections</p>
              </div>
              <div className="text-center">
                <p className="text-2xl font-bold text-indigo-600 dark:text-indigo-400">95%+</p>
                <p className="text-sm text-slate-600 dark:text-slate-400">Recovery Rate</p>
              </div>
            </div>

            <div className="flex flex-col sm:flex-row items-center justify-center gap-4 pt-8 animate-slide-in-up" style={{ animationDelay: '0.3s' }}>
              <Link to="/subscription" className="px-8 py-4 bg-indigo-600 dark:bg-indigo-600 text-white rounded-lg font-semibold shadow-lg hover:shadow-xl hover:bg-indigo-700 dark:hover:bg-indigo-700 hover:-translate-y-1 transition-all duration-300 flex items-center gap-2 whitespace-nowrap group">
                Start Free Trial
                <ArrowRight className="w-5 h-5 group-hover:translate-x-1 transition-transform duration-300" />
              </Link>
              <Link to="/subscription" className="px-8 py-4 border-2 border-slate-300 dark:border-slate-700 text-slate-900 dark:text-white rounded-lg font-semibold bg-white dark:bg-slate-900 hover:bg-slate-50 dark:hover:bg-slate-800 hover:border-slate-400 dark:hover:border-slate-600 transition-all duration-300">
                View Features
              </Link>
            </div>

            <div className="pt-12 text-sm text-slate-600 dark:text-slate-400 animate-fade-in" style={{ animationDelay: '0.4s' }}>
              <p>✓ 1 Month Free  •  ✓ Then ₹299/month  •  ✓ Cancel anytime</p>
            </div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section id="features" className="py-24 md:py-32 px-4 sm:px-6 lg:px-8 bg-gradient-to-b from-slate-50 dark:from-slate-900 via-white dark:via-slate-950 to-white dark:to-slate-950 scroll-mt-40">
        <div className="max-w-6xl mx-auto">
          <div className="text-center mb-20 space-y-4">
            <h2 className="section-heading text-foreground animate-slide-in-up">Everything you need</h2>
            <p className="text-lg text-slate-600 dark:text-slate-300 max-w-2xl mx-auto animate-slide-in-up" style={{ animationDelay: '0.1s' }}>
              Powerful features built for modern society management
            </p>
          </div>

          <div className="card-grid">
            {planFeatures.map((feature, index) => {
              const IconComponent = feature.icon
              return (
                <div
                  key={index}
                  className="p-8 bg-white dark:bg-slate-900 rounded-xl shadow-sm hover:shadow-lg hover:-translate-y-1 border border-slate-200 dark:border-slate-800 hover:border-indigo-200 dark:hover:border-indigo-700 transition-all duration-300 animate-slide-in-up group"
                  style={{ animationDelay: `${0.1 * (index + 1)}s` }}
                >
                  <div className={`w-14 h-14 bg-gradient-to-br ${feature.color} rounded-xl flex items-center justify-center mb-4 shadow-md group-hover:shadow-lg group-hover:scale-110 transition-all duration-300`}>
                    <IconComponent className="w-7 h-7 text-white" />
                  </div>
                  <h3 className="text-lg font-bold text-slate-900 dark:text-white mb-3">{feature.title}</h3>
                  <p className="text-slate-600 dark:text-slate-400 leading-relaxed text-sm">{feature.description}</p>
                </div>
              )
            })}
          </div>
        </div>
      </section>

      {/* Trust & Security Section */}
      <section className="py-20 md:py-28 px-4 sm:px-6 lg:px-8 bg-gradient-to-b from-slate-50 dark:from-slate-900 to-white dark:to-slate-950">
        <div className="max-w-4xl mx-auto">
          <div className="text-center mb-16">
            <h2 className="text-3xl md:text-4xl font-bold text-slate-900 dark:text-white mb-4">Why Choose Us</h2>
            <p className="text-slate-600 dark:text-slate-400">Simple, fast, and trusted by housing societies.</p>
          </div>
          <div className="grid grid-cols-2 md:grid-cols-5 gap-6">
            <div className="text-center">
              <div className="text-3xl mb-2">⚡</div>
              <p className="text-sm font-semibold text-slate-900 dark:text-white">Quick Setup</p>
            </div>
            <div className="text-center">
              <div className="text-3xl mb-2">💵</div>
              <p className="text-sm font-semibold text-slate-900 dark:text-white">Affordable</p>
            </div>
            <div className="text-center">
              <div className="text-3xl mb-2">📱</div>
              <p className="text-sm font-semibold text-slate-900 dark:text-white">Web App</p>
            </div>
            <div className="text-center">
              <div className="text-3xl mb-2">✅</div>
              <p className="text-sm font-semibold text-slate-900 dark:text-white">Trusted Safe</p>
            </div>
          </div>
        </div>
      </section>

      {/* Pricing Section */}
      <section id="pricing" className="py-24 md:py-32 px-4 sm:px-6 lg:px-8 bg-gradient-to-b from-white dark:from-slate-950 via-slate-50 dark:via-slate-900 to-white dark:to-slate-950 scroll-mt-40">
        <div className="max-w-6xl mx-auto">
          <div className="text-center mb-20 space-y-4">
            <h2 className="section-heading text-foreground animate-slide-in-up">Simple, Transparent Pricing</h2>
            <p className="text-lg text-slate-600 dark:text-slate-300 max-w-2xl mx-auto animate-slide-in-up" style={{ animationDelay: '0.1s' }}>
              Start free for 1 month. No credit card required. Cancel anytime.
            </p>
          </div>

          <div className="grid md:grid-cols-1 gap-8 max-w-2xl mx-auto">
            {plansLoading ? (
              <div className="text-center py-12">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600 mx-auto mb-4"></div>
                <p className="text-slate-600 dark:text-slate-400">Loading pricing...</p>
              </div>
            ) : plansError ? (
              <div className="text-center py-12">
                <p className="text-red-600 dark:text-red-400">{plansError}</p>
                <p className="text-slate-600 dark:text-slate-400 mt-2">Please try again later.</p>
              </div>
            ) : (
              pricingPlans.map((plan, index) => (
                <div
                  key={plan.id || index}
                  className={`p-8 rounded-xl border-2 transition-all duration-300 animate-slide-in-up ${
                    plan.highlighted
                      ? "bg-white dark:bg-slate-900 border-indigo-600 shadow-xl relative dark:border-indigo-500"
                      : "bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 hover:border-slate-300 dark:hover:border-slate-700 shadow-sm hover:shadow-lg hover:-translate-y-1"
                  }`}
                  style={{ animationDelay: `${0.1 * (index + 1)}s` }}
                >
                  {plan.highlighted && (
                    <div className="absolute -top-4 left-1/2 transform -translate-x-1/2">
                      <div className="inline-block px-4 py-1 bg-gradient-to-r from-indigo-600 to-indigo-500 text-white rounded-full text-xs font-semibold shadow-lg">
                        Most Popular
                      </div>
                    </div>
                  )}

                  <h3 className="text-2xl font-bold text-slate-900 dark:text-white mb-1">{plan.name}</h3>
                  <p className="text-sm font-semibold text-indigo-600 dark:text-indigo-400 mb-3">{plan.tagline}</p>
                  <p className="text-slate-600 dark:text-slate-400 text-sm mb-6">{plan.description}</p>

                  <div className="mb-8">
                    <div className="flex items-baseline gap-1">
                      <span className="text-5xl font-bold bg-gradient-to-r from-indigo-600 to-indigo-500 dark:from-indigo-400 dark:to-indigo-300 bg-clip-text text-transparent">{plan.price}</span>
                      <span className="text-slate-600 dark:text-slate-400">/month after trial</span>
                    </div>
                    <p className="text-xs text-indigo-600 dark:text-indigo-400 mt-1 font-semibold">{plan.trialText}</p>
                  </div>

                  <ul className="space-y-4 mb-8">
                    {plan.features.map((feature, i) => (
                      <li key={i} className="flex items-start gap-3 text-slate-700 dark:text-slate-300">
                        <ChevronRight className="w-5 h-5 text-indigo-600 dark:text-indigo-400 flex-shrink-0 mt-0.5" />
                        <span className="text-sm">{feature.title}</span>
                      </li>
                    ))}
                  </ul>

                  <Link
                    to="/subscription"
                    aria-label={`Get started with ${plan.name} plan`}
                    className={`block w-full py-3 rounded-lg font-semibold transition-all duration-300 pointer-events-auto text-center ${
                      plan.highlighted
                        ? 'bg-indigo-600 dark:bg-indigo-600 text-white hover:shadow-lg hover:bg-indigo-700 dark:hover:bg-indigo-700 hover:-translate-y-0.5 shadow-lg'
                        : 'bg-slate-100 dark:bg-slate-800 text-indigo-600 dark:text-indigo-400 hover:bg-slate-200 dark:hover:bg-slate-700 border border-slate-200 dark:border-slate-700'
                    }`}>
                    {plan.cta}
                  </Link>
                </div>
              ))
            )}
          </div>
        </div>
      </section>

      {/* FAQ Section */}
      <section id="faq" className="py-20 md:py-28 px-4 sm:px-6 lg:px-8 bg-gradient-to-b from-slate-50 dark:from-slate-900 via-white dark:via-slate-950 to-white dark:to-slate-950 scroll-mt-40">
        <div className="max-w-4xl mx-auto">
          <div className="text-center mb-12">
            <h2 className="text-4xl md:text-5xl font-bold text-slate-900 dark:text-white text-balance">Frequently Asked Questions</h2>
            <p className="text-slate-600 dark:text-slate-400 mt-3">Quick answers to common questions about SocietyLedger and our plans.</p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="p-6 bg-white dark:bg-slate-900 rounded-lg border border-slate-200 dark:border-slate-800 hover:border-indigo-200 dark:hover:border-indigo-700 hover:shadow-md transition-all duration-300">
              <h4 className="font-semibold text-slate-900 dark:text-white mb-2">How does the free trial work?</h4>
              <p className="text-sm text-slate-600 dark:text-slate-400">You get 1 month completely free. No credit card required to start. After 30 days, it's ₹299/month. You can cancel anytime before the trial ends.</p>
            </div>

            <div className="p-6 bg-white dark:bg-slate-900 rounded-lg border border-slate-200 dark:border-slate-800 hover:border-indigo-200 dark:hover:border-indigo-700 hover:shadow-md transition-all duration-300">
              <h4 className="font-semibold text-slate-900 dark:text-white mb-2">Is my resident data secure?</h4>
              <p className="text-sm text-slate-600 dark:text-slate-400">Absolutely. Bank-grade 256-bit encryption, daily automated backups, and regular security audits. Your data is safer than Excel or physical records.</p>
            </div>

            <div className="p-6 bg-white dark:bg-slate-900 rounded-lg border border-slate-200 dark:border-slate-800 hover:border-indigo-200 dark:hover:border-indigo-700 hover:shadow-md transition-all duration-300">
              <h4 className="font-semibold text-slate-900 dark:text-white mb-2">Can I cancel anytime?</h4>
              <p className="text-sm text-slate-600 dark:text-slate-400">Yes, you can cancel your subscription at any time, even during the free trial. No questions asked, no hidden charges. Your data remains accessible.</p>
            </div>

            <div className="p-6 bg-white dark:bg-slate-900 rounded-lg border border-slate-200 dark:border-slate-800 hover:border-indigo-200 dark:hover:border-indigo-700 hover:shadow-md transition-all duration-300">
              <h4 className="font-semibold text-slate-900 dark:text-white mb-2">How do I get support?</h4>
              <p className="text-sm text-slate-600 dark:text-slate-400">Email support is available at support@societyledger.com. We respond within 24 hours to all support requests and help you get the most out of the app.</p>
            </div>
          </div>
        </div>
      </section>

      {/* Testimonials Carousel */}
      <section className="py-24 md:py-32 px-4 sm:px-6 lg:px-8 bg-gradient-to-b from-white dark:from-slate-950 via-slate-50 dark:via-slate-900 to-white dark:to-slate-950">
        <div className="max-w-6xl mx-auto">
          <div className="text-center mb-20 space-y-4">
            <h2 className="text-4xl md:text-5xl font-bold text-slate-900 dark:text-white text-balance animate-slide-in-up">Trusted by 500+ societies</h2>
            <p className="text-lg text-slate-600 dark:text-slate-400 animate-slide-in-up" style={{ animationDelay: '0.1s' }}>See what society managers are saying</p>
          </div>

          <div className="relative">
            {/* Testimonial Cards */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-8 mb-12">
              {testimonials.map((testimonial, index) => (
                <div
                  key={index}
                  className={`p-8 rounded-xl border-2 transition-all duration-300 cursor-pointer animate-slide-in-up ${
                    activeTestimonial === index ? "bg-white dark:bg-slate-900 border-indigo-600 dark:border-indigo-500 shadow-lg scale-105" : "bg-white dark:bg-slate-900 border-slate-200 dark:border-slate-800 hover:shadow-md hover:-translate-y-1"
                  }`}
                  onClick={() => setActiveTestimonial(index)}
                  style={{ animationDelay: `${0.1 * (index + 1)}s` }}
                >
                  <div className="flex gap-1 mb-4">
                    {[...Array(5)].map((_, i) => (
                      <Star key={i} className="w-4 h-4 fill-indigo-400 text-indigo-400" />
                    ))}
                  </div>

                  <p className="text-slate-900 dark:text-white mb-6 leading-relaxed">"{testimonial.text}"</p>

                  <div className="flex items-center gap-4">
                    <div className="text-3xl">{testimonial.image}</div>
                    <div>
                      <p className="font-semibold text-slate-900 dark:text-white">{testimonial.name}</p>
                      <p className="text-sm text-slate-600 dark:text-slate-400">
                        {testimonial.role} • {testimonial.society}
                      </p>
                    </div>
                  </div>
                </div>
              ))}
            </div>

            {/* Dots */}
            <div className="flex justify-center gap-3 animate-fade-in" style={{ animationDelay: '0.4s' }}>
              {testimonials.map((_, index) => (
                <button
                  key={index}
                  onClick={() => setActiveTestimonial(index)}
                  className={`rounded-full transition-all duration-300 ${
                    activeTestimonial === index ? "bg-indigo-600 dark:bg-indigo-500 w-8 h-2.5" : "bg-indigo-200 dark:bg-indigo-800 w-2.5 h-2.5 hover:bg-indigo-300 dark:hover:bg-indigo-700"
                  }`}
                  aria-label={`View testimonial ${index + 1}`}
                />
              ))}
            </div>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-24 md:py-32 px-4 sm:px-6 lg:px-8 relative overflow-hidden bg-white dark:bg-slate-950">
        <div className="absolute inset-0 bg-gradient-to-br from-indigo-50 dark:from-indigo-950/20 to-transparent pointer-events-none" />
        <div className="max-w-4xl mx-auto relative z-10">
          <div className="p-12 md:p-16 bg-gradient-to-br from-indigo-600 to-indigo-700 dark:from-indigo-600 dark:to-indigo-700 rounded-2xl shadow-xl text-center space-y-6 border border-indigo-500/50 hover:shadow-2xl hover:border-indigo-400/50 transition-all duration-300">
            <h2 className="text-4xl md:text-5xl font-bold text-white animate-slide-in-up">Start your free trial today</h2>
            <p className="text-lg text-white/90 animate-slide-in-up" style={{ animationDelay: '0.1s' }}>
              1 month free, no credit card required. Join 500+ societies managing with SocietyLedger.
            </p>
            <Link to="/free-trial" className="px-8 py-4 bg-white text-indigo-600 rounded-lg font-semibold shadow-lg hover:shadow-xl hover:bg-slate-50 hover:-translate-y-1 transition-all duration-300 flex items-center justify-center gap-2 mx-auto group animate-slide-in-up" style={{ animationDelay: '0.2s' }}>
              Start Free Trial
              <ArrowRight className="w-5 h-5 group-hover:translate-x-1 transition-transform duration-300" />
            </Link>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="border-t border-slate-200 dark:border-slate-800 bg-gradient-to-b from-white dark:from-slate-950 to-slate-50 dark:to-slate-900">
        <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
         
          <div className="grid grid-cols-2 md:grid-cols-4 gap-8 mb-12">
            <div>
              <h4 className="font-bold text-slate-900 dark:text-white mb-4 text-sm uppercase tracking-wide">Product</h4>
              <ul className="space-y-2 text-sm text-slate-600 dark:text-slate-400">
                <li>
                  <a href="#" className="hover:text-indigo-600 dark:hover:text-indigo-400 transition-colors duration-300 font-medium">
                    Features
                  </a>
                </li>
                <li>
                  <a href="#" className="hover:text-indigo-600 dark:hover:text-indigo-400 transition-colors duration-300 font-medium">
                    Pricing
                  </a>
                </li>
                <li>
                  <a href="#" className="hover:text-indigo-600 dark:hover:text-indigo-400 transition-colors duration-300 font-medium">
                    Security
                  </a>
                </li>
              </ul>
            </div>
            <div>
              <h4 className="font-bold text-slate-900 dark:text-white mb-4 text-sm uppercase tracking-wide">Support</h4>
              <ul className="space-y-2 text-sm text-slate-600 dark:text-slate-400">
                <li>
                  <a href="#" className="hover:text-indigo-600 dark:hover:text-indigo-400 transition-colors duration-300 font-medium">
                    Help Center
                  </a>
                </li>
                <li>
                  <a href="#" className="hover:text-indigo-600 dark:hover:text-indigo-400 transition-colors duration-300 font-medium">
                    Contact
                  </a>
                </li>
                <li>
                  <a href="#" className="hover:text-indigo-600 dark:hover:text-indigo-400 transition-colors duration-300 font-medium">
                    Status
                  </a>
                </li>
              </ul>
            </div>
            <div>
              <h4 className="font-bold text-slate-900 dark:text-white mb-4 text-sm uppercase tracking-wide">Company</h4>
              <ul className="space-y-2 text-sm text-slate-600 dark:text-slate-400">
                <li>
                  <a href="#" className="hover:text-indigo-600 dark:hover:text-indigo-400 transition-colors duration-300 font-medium">
                    About
                  </a>
                </li>
                <li>
                  <a href="#" className="hover:text-indigo-600 dark:hover:text-indigo-400 transition-colors duration-300 font-medium">
                    Blog
                  </a>
                </li>
                <li>
                  <a href="#" className="hover:text-indigo-600 dark:hover:text-indigo-400 transition-colors duration-300 font-medium">
                    Careers
                  </a>
                </li>
              </ul>
            </div>
            <div>
              <h4 className="font-bold text-slate-900 dark:text-white mb-4 text-sm uppercase tracking-wide">Legal</h4>
              <ul className="space-y-2 text-sm text-slate-600 dark:text-slate-400">
                <li>
                  <a href="#" className="hover:text-indigo-600 dark:hover:text-indigo-400 transition-colors duration-300 font-medium">
                    Privacy
                  </a>
                </li>
                <li>
                  <a href="#" className="hover:text-indigo-600 dark:hover:text-indigo-400 transition-colors duration-300 font-medium">
                    Terms
                  </a>
                </li>
                <li>
                  <a href="#" className="hover:text-indigo-600 dark:hover:text-indigo-400 transition-colors duration-300 font-medium">
                    Cookies
                  </a>
                </li>
              </ul>
            </div>
          </div>
          <div className="pt-8 border-t border-slate-200 dark:border-slate-800 text-center text-sm text-slate-600 dark:text-slate-400">
            <p>&copy; 2025 SocietyLedger. All rights reserved.</p>
          </div>
        </div>
      </footer>
    </div>
  )
}
