import { ApiResponse } from '../types/api';
import apiClient from './client';

// Types for subscription API
export interface TrialResponse {
  succeeded: true;
  message: string;
  trialEnd: string;
}

export interface TrialError {
  succeeded: false;
  error: 'ALREADY_HAS_SUBSCRIPTION' | 'NO_DEFAULT_PLAN';
}

export interface SubscriptionStatusResponse {
  succeeded: true;
  data: {
    accessAllowed: boolean;
    status: 'trial' | 'active' | 'expired' | 'cancelled';
    trialDaysRemaining?: number;
    planName: string;
    monthlyAmount: number;
  };
}

export interface SubscriptionStatusError {
  succeeded: false;
  error: 'USER_NOT_FOUND';
}

export interface SubscribeRequest {
  planId: string;
  amount: number;
  paymentMethod: string;
  paymentReference: string;
}

export interface SubscribeResponse {
  succeeded: true;
  data: {
    subscriptionId: string;
    invoiceId: string;
    status: 'active';
  };
  message: string;
}

export interface SubscribeError {
  succeeded: false;
  error: 'INVALID_PLAN' | 'ALREADY_SUBSCRIBED';
}

export interface CancelSubscriptionRequest {
  reason: string;
}

export interface CancelSubscriptionResponse {
  succeeded: true;
  message: string;
}

export interface CancelSubscriptionError {
  succeeded: false;
  error: 'NO_ACTIVE_SUBSCRIPTION';
}

export const subscriptionApi = {
  /**
   * Create a trial subscription for a new user
   * Automatically called on user registration/login
   * @returns Promise<TrialResponse | TrialError>
   */
  async createTrial(): Promise<TrialResponse | TrialError> {
    try {
      const response = await apiClient.post<ApiResponse<TrialResponse>>('/subscriptions/trial');
      if (response.data.succeeded) {
        return response.data.data;
      } else {
        return {
          succeeded: false,
          error: 'ALREADY_HAS_SUBSCRIPTION'
        };
      }
    } catch (error: any) {
      // Handle specific error cases
      if (error.response?.status === 400) {
        return {
          succeeded: false,
          error: error.response.data?.error || 'ALREADY_HAS_SUBSCRIPTION'
        };
      }
      throw error;
    }
  },

  /**
   * Get current user's subscription status
   * Call on app load or feature access to check permissions
   * @returns Promise<SubscriptionStatusResponse | SubscriptionStatusError>
   */
  async getStatus(): Promise<SubscriptionStatusResponse | SubscriptionStatusError> {
    try {
      const response = await apiClient.get<ApiResponse<SubscriptionStatusResponse['data']>>('/subscriptions/status');
      if (response.data.succeeded) {
        return {
          succeeded: true,
          data: response.data.data
        };
      } else {
        return {
          succeeded: false,
          error: 'USER_NOT_FOUND'
        };
      }
    } catch (error: any) {
      if (error.response?.status === 404) {
        return {
          succeeded: false,
          error: 'USER_NOT_FOUND'
        };
      }
      throw error;
    }
  },

  /**
   * Subscribe to a plan after payment completion
   * Call after successful payment (Stripe/Razorpay callback)
   * @param request Subscription details
   * @returns Promise<SubscribeResponse | SubscribeError>
   */
  async subscribe(request: SubscribeRequest): Promise<SubscribeResponse | SubscribeError> {
    try {
      const response = await apiClient.post<ApiResponse<SubscribeResponse['data']>>('/subscriptions/subscribe', request);
      if (response.data.succeeded) {
        return {
          succeeded: true,
          data: response.data.data,
          message: response.data.message
        };
      } else {
        return {
          succeeded: false,
          error: 'INVALID_PLAN'
        };
      }
    } catch (error: any) {
      if (error.response?.status === 400) {
        return {
          succeeded: false,
          error: error.response.data?.error || 'INVALID_PLAN'
        };
      }
      throw error;
    }
  },

  /**
   * Cancel the current active subscription
   * Call from settings page after user confirmation
   * @param request Cancellation reason
   * @returns Promise<CancelSubscriptionResponse | CancelSubscriptionError>
   */
  async cancel(request: CancelSubscriptionRequest): Promise<CancelSubscriptionResponse | CancelSubscriptionError> {
    try {
      const response = await apiClient.post<ApiResponse<null>>('/subscriptions/cancel', request);
      if (response.data.succeeded) {
        return {
          succeeded: true,
          message: response.data.message
        };
      } else {
        return {
          succeeded: false,
          error: 'NO_ACTIVE_SUBSCRIPTION'
        };
      }
    } catch (error: any) {
      if (error.response?.status === 400) {
        return {
          succeeded: false,
          error: 'NO_ACTIVE_SUBSCRIPTION'
        };
      }
      throw error;
    }
  }
};