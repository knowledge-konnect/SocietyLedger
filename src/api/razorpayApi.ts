import { ApiResponse } from '../types/api';
import apiClient from './client';

/**
 * Razorpay Order Request
 */
export interface CreateRazorpayOrderRequest {
  planId: string;
  amount: number; // Amount in rupees (will be converted to paise by backend)
  currency?: string; // Default: 'INR'
}

/**
 * Razorpay Order Response
 */
export interface RazorpayOrderResponse {
  succeeded: true;
  data: {
    orderId: string; // Razorpay Order ID
    keyId: string; // Razorpay Key ID (public)
    amount: number; // Amount in paise
    currency: string;
  };
  message: string;
}

export interface RazorpayOrderError {
  succeeded: false;
  error: string;
}

/**
 * Payment Verification Request
 */
export interface VerifyPaymentRequest {
  razorpay_order_id: string;
  razorpay_payment_id: string;
  razorpay_signature: string;
  planId: string;
}

/**
 * Payment Verification Response
 */
export interface VerifyPaymentResponse {
  succeeded: true;
  data: {
    subscriptionId: string;
    status: 'active';
    message: string;
  };
}

export interface VerifyPaymentError {
  succeeded: false;
  error: string;
}

/**
 * Razorpay Payment APIs
 */
export const razorpayApi = {
  /**
   * Create a Razorpay order for subscription payment
   * @param request Order creation details
   * @returns Promise with orderId and keyId
   */
  async createOrder(
    request: CreateRazorpayOrderRequest
  ): Promise<RazorpayOrderResponse | RazorpayOrderError> {
    try {
      const response = await apiClient.post<
        ApiResponse<RazorpayOrderResponse['data']>
      >('/payments/create-order', request);

      if (response.data.succeeded) {
        return {
          succeeded: true,
          data: response.data.data,
          message: response.data.message,
        };
      } else {
        return {
          succeeded: false,
          error: 'Failed to create order',
        };
      }
    } catch (error: any) {
      return {
        succeeded: false,
        error: error.response?.data?.error || error.message || 'Failed to create order',
      };
    }
  },

  /**
   * Verify Razorpay payment with backend
   * CRITICAL: Do NOT trust frontend payment success - always verify with backend
   * @param request Payment details to verify
   * @returns Promise with verification result
   */
  async verifyPayment(
    request: VerifyPaymentRequest
  ): Promise<VerifyPaymentResponse | VerifyPaymentError> {
    try {
      const response = await apiClient.post<
        ApiResponse<VerifyPaymentResponse['data']>
      >('/payments/verify-payment', request);

      if (response.data.succeeded) {
        return {
          succeeded: true,
          data: response.data.data,
        };
      } else {
        return {
          succeeded: false,
          error: 'Payment verification failed',
        };
      }
    } catch (error: any) {
      return {
        succeeded: false,
        error: error.response?.data?.error || error.message || 'Payment verification failed',
      };
    }
  },
};