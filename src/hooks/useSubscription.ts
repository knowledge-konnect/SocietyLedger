import { useState, useEffect, useCallback } from 'react';
import { subscriptionApi, SubscriptionStatusResponse, TrialResponse, SubscribeResponse, CancelSubscriptionResponse } from '../api/subscriptionApi';

interface SubscriptionState {
  accessAllowed: boolean;
  status: 'trial' | 'active' | 'expired' | 'cancelled' | null;
  trialDaysRemaining: number | null;
  planName: string | null;
  monthlyAmount: number | null;
  trialEnd: string | null;
  loading: boolean;
  error: string | null;
}

interface UseSubscriptionReturn extends SubscriptionState {
  createTrial: () => Promise<TrialResponse | null>;
  subscribe: (planId: string, amount: number, paymentMethod: string, paymentReference: string) => Promise<SubscribeResponse | null>;
  cancelSubscription: (reason: string) => Promise<CancelSubscriptionResponse | null>;
  refreshStatus: () => Promise<void>;
  clearError: () => void;
}

const initialState: SubscriptionState = {
  accessAllowed: false,
  status: null,
  trialDaysRemaining: null,
  planName: null,
  monthlyAmount: null,
  trialEnd: null,
  loading: false,
  error: null,
};

/**
 * Custom hook for managing user subscription state
 * Handles trial creation, status checking, subscription management
 * Integrates with subscription API and manages loading/error states
 */
export function useSubscription(): UseSubscriptionReturn {
  const [state, setState] = useState<SubscriptionState>(initialState);

  /**
   * Update subscription state from API response
   */
  const updateSubscriptionState = useCallback((data: SubscriptionStatusResponse['data']) => {
    setState(prev => ({
      ...prev,
      accessAllowed: data.accessAllowed,
      status: data.status,
      trialDaysRemaining: data.trialDaysRemaining || null,
      planName: data.planName,
      monthlyAmount: data.monthlyAmount,
      loading: false,
      error: null,
    }));
  }, []);

  /**
   * Handle API errors with retry logic for 500 errors
   */
  const handleApiError = useCallback(async (error: any, retryFn?: () => Promise<any>, maxRetries = 3): Promise<any> => {
    if (error.response?.status === 401) {
      // Redirect to login on auth errors
      window.location.href = '/login?reason=session_expired';
      return null;
    }

    if (error.response?.status >= 500 && retryFn && maxRetries > 0) {
      // Retry logic for server errors
      setState(prev => ({ ...prev, loading: true, error: 'Retrying...' }));
      await new Promise(resolve => setTimeout(resolve, 1000)); // Wait 1 second
      return handleApiError(await retryFn(), retryFn, maxRetries - 1);
    }

    const errorMessage = error.response?.data?.message || error.message || 'An error occurred';
    setState(prev => ({ ...prev, loading: false, error: errorMessage }));
    return null;
  }, []);

  /**
   * Refresh subscription status from API
   */
  const refreshStatus = useCallback(async () => {
    setState(prev => ({ ...prev, loading: true, error: null }));

    try {
      const response = await subscriptionApi.getStatus();
      if (response.succeeded) {
        updateSubscriptionState(response.data);
      } else {
        setState(prev => ({
          ...prev,
          loading: false,
          error: `Failed to get subscription status: ${response.error}`
        }));
      }
    } catch (error) {
      await handleApiError(error, refreshStatus);
    }
  }, [updateSubscriptionState, handleApiError]);

  /**
   * Create trial subscription
   * Automatically called on user registration
   */
  const createTrial = useCallback(async (): Promise<TrialResponse | null> => {
    setState(prev => ({ ...prev, loading: true, error: null }));

    try {
      const response = await subscriptionApi.createTrial();
      if (response.succeeded) {
        setState(prev => ({
          ...prev,
          trialEnd: response.trialEnd,
          loading: false,
          error: null
        }));
        // Refresh status to get updated state
        await refreshStatus();
        return response;
      } else {
        setState(prev => ({
          ...prev,
          loading: false,
          error: `Trial creation failed: ${response.error}`
        }));
        return null;
      }
    } catch (error) {
      return await handleApiError(error, () => subscriptionApi.createTrial());
    }
  }, [refreshStatus, handleApiError]);

  /**
   * Subscribe to a plan after payment
   */
  const subscribe = useCallback(async (
    planId: string,
    amount: number,
    paymentMethod: string,
    paymentReference: string
  ): Promise<SubscribeResponse | null> => {
    setState(prev => ({ ...prev, loading: true, error: null }));

    try {
      const response = await subscriptionApi.subscribe({
        planId,
        amount,
        paymentMethod,
        paymentReference
      });

      if (response.succeeded) {
        setState(prev => ({ ...prev, loading: false, error: null }));
        // Refresh status to get updated subscription info
        await refreshStatus();
        return response;
      } else {
        setState(prev => ({
          ...prev,
          loading: false,
          error: `Subscription failed: ${response.error}`
        }));
        return null;
      }
    } catch (error) {
      return await handleApiError(error, () => subscriptionApi.subscribe({
        planId, amount, paymentMethod, paymentReference
      }));
    }
  }, [refreshStatus, handleApiError]);

  /**
   * Cancel current subscription
   */
  const cancelSubscription = useCallback(async (reason: string): Promise<CancelSubscriptionResponse | null> => {
    setState(prev => ({ ...prev, loading: true, error: null }));

    try {
      const response = await subscriptionApi.cancel({ reason });
      if (response.succeeded) {
        setState(prev => ({ ...prev, loading: false, error: null }));
        // Refresh status to reflect cancellation
        await refreshStatus();
        return response;
      } else {
        setState(prev => ({
          ...prev,
          loading: false,
          error: `Cancellation failed: ${response.error}`
        }));
        return null;
      }
    } catch (error) {
      return await handleApiError(error, () => subscriptionApi.cancel({ reason }));
    }
  }, [refreshStatus, handleApiError]);

  /**
   * Clear error state
   */
  const clearError = useCallback(() => {
    setState(prev => ({ ...prev, error: null }));
  }, []);

  // Load subscription status on mount
  useEffect(() => {
    refreshStatus();
  }, [refreshStatus]);

  return {
    ...state,
    createTrial,
    subscribe,
    cancelSubscription,
    refreshStatus,
    clearError,
  };
}

// Legacy interface for backward compatibility
export interface SubscriptionStatus {
  hasAccess: boolean
  status: 'trial' | 'active' | 'expired' | 'past_due' | 'cancelled'
  trialDaysRemaining: number
  subscriptionEndDate?: Date
  nextBillingDate?: Date
  planName?: string
  monthlyAmount?: number
}

export interface UserSubscription {
  id: string
  trial_start_date?: string
  trial_ends_date?: string
  subscription_status: string
  subscription_start_date?: string
  next_billing_date?: string
  subscription_plan?: string
  monthly_amount?: number
}

/**
 * Legacy hook for backward compatibility
 * @deprecated Use useSubscription instead
 */
export const useSubscriptionCheck = (userId?: string) => {
  const subscription = useSubscription();

  // Map new state to legacy interface
  const subscriptionStatus: SubscriptionStatus = {
    hasAccess: subscription.accessAllowed,
    status: (subscription.status as any) || 'trial',
    trialDaysRemaining: subscription.trialDaysRemaining || 0,
    planName: subscription.planName || undefined,
    monthlyAmount: subscription.monthlyAmount || undefined,
  };

  return {
    subscriptionStatus,
    loading: subscription.loading,
    error: subscription.error,
    refetch: subscription.refreshStatus,
  };
};