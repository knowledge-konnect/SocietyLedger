/**
 * Razorpay Payment Integration
 * Handles loading of Razorpay Checkout script and payment utilities
 */

let razorpayLoaded = false;

/**
 * Load Razorpay Checkout Script
 * Dynamically loads the script only once
 * @returns Promise that resolves when script is loaded
 */
export const loadRazorpayScript = (): Promise<void> => {
  return new Promise((resolve, reject) => {
    // Check if already loaded
    if (razorpayLoaded || (window as any).Razorpay) {
      resolve();
      return;
    }

    // Check if script is already in DOM
    const existingScript = document.getElementById('razorpay-checkout-js');
    if (existingScript) {
      razorpayLoaded = true;
      resolve();
      return;
    }

    // Create and append script
    const script = document.createElement('script');
    script.id = 'razorpay-checkout-js';
    script.src = 'https://checkout.razorpay.com/v1/checkout.js';
    script.async = true;

    script.onload = () => {
      razorpayLoaded = true;
      resolve();
    };

    script.onerror = () => {
      reject(new Error('Failed to load Razorpay Checkout script'));
    };

    document.body.appendChild(script);
  });
};

/**
 * Razorpay Payment Configuration
 */
export interface RazorpayPaymentConfig {
  key: string; // Razorpay Key ID
  order_id: string; // Order ID from backend
  amount: number; // Amount in paise (e.g., ₹100 = 10000)
  currency: string; // Currency code (e.g., 'INR')
  name: string; // Organization name
  description: string; // Payment description
  prefill?: {
    email: string;
    contact: string;
    name: string;
  };
  notes?: Record<string, string>;
}

/**
 * Razorpay Payment Response
 */
export interface RazorpayPaymentResponse {
  razorpay_payment_id: string;
  razorpay_order_id: string;
  razorpay_signature: string;
}

/**
 * Open Razorpay Checkout Modal
 * @param config Payment configuration
 * @param onSuccess Callback on successful payment
 * @param onError Callback on payment error or dismissal
 */
export const openRazorpayCheckout = async (
  config: RazorpayPaymentConfig,
  onSuccess: (response: RazorpayPaymentResponse) => void,
  onError: (error: any) => void
) => {
  try {
    await loadRazorpayScript();

    const options = {
      key: config.key,
      order_id: config.order_id,
      amount: config.amount,
      currency: config.currency,
      name: config.name,
      description: config.description,
      prefill: config.prefill || {},
      notes: config.notes || {},
      handler: (response: RazorpayPaymentResponse) => {
        onSuccess(response);
      },
      modal: {
        ondismiss: () => {
          onError(new Error('Payment cancelled by user'));
        },
      },
    };

    const razorpay = new (window as any).Razorpay(options);
    razorpay.open();
  } catch (error) {
    onError(error);
  }
};