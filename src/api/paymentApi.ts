import client from './client'

export interface PaymentPlan {
  id: 'basic' | 'standard' | 'pro'
  name: string
  price: number
  originalPrice?: number
  period: 'monthly' | 'yearly'
  features: string[]
  popular?: boolean
}

export interface CreateOrderRequest {
  planId: PaymentPlan['id']
  amount: number
  currency: string
}

export interface CreateOrderResponse {
  id: string
  amount: number
  currency: string
}

export interface VerifyPaymentRequest {
  razorpay_order_id: string
  razorpay_payment_id: string
  razorpay_signature: string
  planId: PaymentPlan['id']
}

export interface VerifyPaymentResponse {
  success: boolean
  message: string
}

// Payment plans configuration
export const PAYMENT_PLANS: PaymentPlan[] = [
  {
    id: 'basic',
    name: 'Basic',
    price: 249,
    period: 'monthly',
    features: [
      'Up to 50 flats',
      'Basic financial tracking',
      'Announcement management',
      'Document storage (1GB)',
      'Email support'
    ]
  },
  {
    id: 'standard',
    name: 'Standard',
    price: 499,
    originalPrice: 599,
    period: 'monthly',
    popular: true,
    features: [
      'Up to 200 flats',
      'Advanced financial reports',
      'Maintenance tracking',
      'Document storage (5GB)',
      'Priority email support',
      'API access'
    ]
  },
  {
    id: 'pro',
    name: 'Pro',
    price: 999,
    period: 'monthly',
    features: [
      'Unlimited flats',
      'Custom reports & analytics',
      'Advanced maintenance workflows',
      'Document storage (25GB)',
      'Phone & priority support',
      'White-label option',
      'Custom integrations'
    ]
  }
]

export const paymentApi = {
  // Create Razorpay order
  createOrder: async (data: CreateOrderRequest): Promise<CreateOrderResponse> => {
    const response = await client.post('/create-order', data)
    return response.data
  },

  // Verify payment after completion
  verifyPayment: async (data: VerifyPaymentRequest): Promise<VerifyPaymentResponse> => {
    const response = await client.post('/verify-payment', data)
    return response.data
  },

  // Get plan by ID
  getPlanById: (planId: PaymentPlan['id']): PaymentPlan | undefined => {
    return PAYMENT_PLANS.find(plan => plan.id === planId)
  }
}