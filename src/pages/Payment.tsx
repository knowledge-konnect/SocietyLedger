"use client"

import { useEffect, useState } from 'react'
import { useSearchParams, useNavigate } from 'react-router-dom'
import Navbar from '../components/layout/Navbar'
import { ArrowLeft, CheckCircle, AlertCircle, Loader } from 'lucide-react'
import { paymentApi } from '@/api/paymentApi'
import { useToast } from '@/hooks/use-toast'

declare global {
  interface Window {
    Razorpay: any
  }
}

export default function Payment() {
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const [isProcessing, setIsProcessing] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const planId = searchParams.get('plan') as 'basic' | 'standard' | 'pro' || 'standard'

  const plan = paymentApi.getPlanById(planId) || paymentApi.getPlanById('standard')!

  // Load Razorpay script
  useEffect(() => {
    const script = document.createElement('script')
    script.src = 'https://checkout.razorpay.com/v1/checkout.js'
    script.async = true
    document.body.appendChild(script)

    return () => {
      document.body.removeChild(script)
    }
  }, [])

  const handleRazorpayPayment = async () => {
    if (!window.Razorpay) {
      setError('Razorpay failed to load. Please refresh and try again.')
      return
    }

    setIsProcessing(true)
    setError(null)

    try {
      // Call backend to create order using paymentApi
      const orderData = await paymentApi.createOrder({
        planId,
        amount: plan.price * 100, // Convert to paise
        currency: 'INR',
      })

      const options = {
        key: process.env.REACT_APP_RAZORPAY_KEY_ID || '',
        amount: orderData.amount,
        currency: orderData.currency,
        order_id: orderData.id,
        name: 'SocietyLedger',
        description: `${plan.name} Plan Subscription`,
        handler: async (response: any) => {
          try {
            // Verify payment on backend using paymentApi
            await paymentApi.verifyPayment({
              razorpay_order_id: response.razorpay_order_id,
              razorpay_payment_id: response.razorpay_payment_id,
              razorpay_signature: response.razorpay_signature,
              planId,
            })

            // Payment successful
            toast({
              title: 'Payment Successful!',
              description: 'Your subscription has been activated.',
            })
            navigate('/payment-success', { state: { planId, orderId: response.razorpay_order_id } })
          } catch (err) {
            console.error('Payment verification failed:', err)
            toast({
              title: 'Payment Verification Failed',
              description: 'Please contact support if amount was debited.',
              variant: 'destructive',
            })
          } finally {
            setIsProcessing(false)
          }
        },
        prefill: {
          email: localStorage.getItem('userEmail') || '',
          contact: localStorage.getItem('userPhone') || '',
        },
        theme: {
          color: '#3b82f6', // Primary color
        },
      }

      const rzp1 = new window.Razorpay(options)
      rzp1.on('payment.failed', (response: any) => {
        setError(`Payment failed: ${response.error.description}`)
        setIsProcessing(false)
        toast({
          title: 'Payment Failed',
          description: response.error.description,
          variant: 'destructive',
        })
      })
      rzp1.open()
    } catch (err) {
      console.error('Payment error:', err)
      const errorMessage = err instanceof Error ? err.message : 'An error occurred'
      setError(errorMessage)
      setIsProcessing(false)
      toast({
        title: 'Payment Error',
        description: errorMessage,
        variant: 'destructive',
      })
    }
  }

  return (
    <div className="min-h-screen bg-background">
      <Navbar variant="landing" />

      <div className="py-12 px-4 sm:px-6 lg:px-8">
        <div className="max-w-2xl mx-auto">
          {/* Header */}
          <button
            onClick={() => navigate('/subscription')}
            className="mb-8 flex items-center gap-2 text-primary hover:text-primary/80 transition-colors"
          >
            <ArrowLeft className="w-4 h-4" />
            Back to plans
          </button>

          {/* Main Card */}
          <div className="bg-card rounded-2xl border border-border shadow-lg overflow-hidden">
            {/* Order Summary */}
            <div className="bg-gradient-to-br from-primary/5 to-secondary/5 border-b border-border p-8">
              <h1 className="text-3xl font-bold text-foreground mb-2">Confirm Your Subscription</h1>
              <p className="text-muted-foreground">Review and complete your payment</p>
            </div>

            {/* Content */}
            <div className="p-8">
              {error && (
                <div className="mb-6 p-4 bg-destructive/10 border border-destructive/30 rounded-lg flex items-start gap-3">
                  <AlertCircle className="w-5 h-5 text-destructive flex-shrink-0 mt-0.5" />
                  <div>
                    <p className="font-semibold text-destructive">Payment Error</p>
                    <p className="text-sm text-destructive/80">{error}</p>
                  </div>
                </div>
              )}

              <div className="space-y-8">
                {/* Plan Details */}
                <div className="bg-background rounded-xl border border-border p-6">
                  <div className="flex justify-between items-start mb-4">
                    <div>
                      <h3 className="text-2xl font-bold text-foreground">{plan.name} Plan</h3>
                      <p className="text-sm text-muted-foreground mt-1">
                        {plan.features.slice(0, 2).join(' • ')}
                      </p>
                    </div>
                  </div>

                  <div className="border-t border-border pt-4 mt-4">
                    <div className="flex justify-between items-center mb-3">
                      <span className="text-muted-foreground">Monthly subscription</span>
                      <span className="font-semibold text-foreground">₹{plan.price}</span>
                    </div>
                    <div className="flex justify-between items-center text-sm text-muted-foreground">
                      <span>Tax & fees</span>
                      <span>Included</span>
                    </div>
                  </div>

                  <div className="border-t border-border pt-4 mt-4">
                    <div className="flex justify-between items-center">
                      <span className="font-semibold text-foreground">Total due today</span>
                      <span className="text-3xl font-bold bg-gradient-to-r from-primary to-secondary bg-clip-text text-transparent">
                        ₹{plan.price}
                      </span>
                    </div>
                  </div>
                </div>

                {/* Payment Methods */}
                <div className="space-y-3">
                  <h4 className="font-semibold text-foreground">Select Payment Method</h4>

                  <button
                    onClick={handleRazorpayPayment}
                    disabled={isProcessing}
                    className="w-full p-4 border-2 border-primary rounded-xl hover:bg-primary/5 transition-all duration-300 flex items-center justify-between group disabled:opacity-70 disabled:cursor-not-allowed"
                  >
                    <div className="flex items-center gap-3">
                      <div className="w-10 h-10 bg-blue-600 rounded-lg flex items-center justify-center text-white font-bold text-lg">
                        ₹
                      </div>
                      <div className="text-left">
                        <p className="font-semibold text-foreground">Razorpay</p>
                        <p className="text-xs text-muted-foreground">Credit/Debit Card, UPI, Wallets</p>
                      </div>
                    </div>
                    {isProcessing ? (
                      <Loader className="w-5 h-5 text-primary animate-spin" />
                    ) : (
                      <ArrowLeft className="w-5 h-5 text-primary rotate-180 group-hover:translate-x-1 transition-transform" />
                    )}
                  </button>
                </div>

                {/* T&C */}
                <div className="p-4 bg-background rounded-lg border border-border/50">
                  <p className="text-xs text-muted-foreground leading-relaxed">
                    By proceeding with payment, you agree to our{' '}
                    <a href="#" className="text-primary hover:underline">
                      Terms of Service
                    </a>{' '}
                    and{' '}
                    <a href="#" className="text-primary hover:underline">
                      Privacy Policy
                    </a>
                    . Your subscription will renew automatically on the same date each month. You can cancel anytime from your account settings.
                  </p>
                </div>

                {/* CTA Button */}
                <button
                  onClick={handleRazorpayPayment}
                  disabled={isProcessing}
                  className="w-full py-4 px-6 bg-gradient-to-r from-primary to-secondary text-white rounded-xl font-semibold shadow-lg hover:shadow-2xl hover:-translate-y-1 transition-all duration-300 disabled:opacity-70 disabled:cursor-not-allowed flex items-center justify-center gap-2"
                >
                  {isProcessing ? (
                    <>
                      <Loader className="w-5 h-5 animate-spin" />
                      Processing Payment...
                    </>
                  ) : (
                    <>
                      <CheckCircle className="w-5 h-5" />
                      Pay ₹{plan.price} & Activate Subscription
                    </>
                  )}
                </button>
              </div>
            </div>

            {/* Security Footer */}
            <div className="bg-background border-t border-border px-8 py-4">
              <div className="flex items-center justify-center gap-6 text-xs text-muted-foreground">
                <span>🔒 Secure Payment</span>
                <span>•</span>
                <span>SSL Encrypted</span>
                <span>•</span>
                <span>PCI DSS Compliant</span>
              </div>
            </div>
          </div>

          {/* Trust Signals */}
          <div className="mt-8 grid md:grid-cols-3 gap-4">
            <div className="text-center">
              <div className="text-3xl mb-2">✓</div>
              <p className="text-sm text-muted-foreground">
                <span className="font-semibold text-foreground">Easy Cancellation</span>
                <br />
                Cancel anytime, no questions asked
              </p>
            </div>
            <div className="text-center">
              <div className="text-3xl mb-2">💳</div>
              <p className="text-sm text-muted-foreground">
                <span className="font-semibold text-foreground">Secure Payment</span>
                <br />
                Bank-grade encryption
              </p>
            </div>
            <div className="text-center">
              <div className="text-3xl mb-2">🎁</div>
              <p className="text-sm text-muted-foreground">
                <span className="font-semibold text-foreground">Money-back Guarantee</span>
                <br />
                7-day refund guarantee
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
