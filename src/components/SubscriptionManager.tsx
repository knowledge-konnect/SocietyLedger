import React, { useEffect } from 'react';
import { useSubscription } from '../hooks/useSubscription';
import Button from '../components/ui/Button';
import Card, { CardContent, CardHeader, CardTitle } from '../components/ui/Card';
import CardDescription from '../components/ui/Card';
import Badge from '../components/ui/Badge';
import { Alert, AlertDescription } from '../components/ui/Alert';
import { Loader2, CheckCircle, XCircle, AlertTriangle, CreditCard, Calendar } from 'lucide-react';

interface SubscriptionManagerProps {
  onTrialCreated?: (trialEnd: string) => void;
  onSubscribed?: (subscriptionId: string) => void;
  onCancelled?: () => void;
}

/**
 * SubscriptionManager Component
 *
 * A comprehensive React component for managing user subscriptions
 * Features:
 * - Displays current subscription status
 * - Handles trial creation
 * - Manages subscription actions (subscribe/cancel)
 * - Error handling with retry logic
 * - Loading states and user feedback
 * - Error boundaries for graceful failure handling
 */
export const SubscriptionManager: React.FC<SubscriptionManagerProps> = ({
  onTrialCreated,
  onSubscribed,
  onCancelled,
}) => {
  const {
    accessAllowed,
    status,
    trialDaysRemaining,
    planName,
    monthlyAmount,
    trialEnd,
    loading,
    error,
    createTrial,
    subscribe,
    cancelSubscription,
    refreshStatus,
    clearError,
  } = useSubscription();

  // Handle trial creation on component mount if no subscription exists
  useEffect(() => {
    if (status === null && !loading) {
      // Automatically create trial for new users
      handleCreateTrial();
    }
  }, [status, loading]);

  const handleCreateTrial = async () => {
    try {
      const result = await createTrial();
      if (result?.succeeded) {
        onTrialCreated?.(result.trialEnd);
      }
    } catch (error) {
      console.error('Error creating trial:', error);
    }
  };

  const handleSubscribe = async (planId: string, amount: number, paymentMethod: string, paymentReference: string) => {
    try {
      const result = await subscribe(planId, amount, paymentMethod, paymentReference);
      if (result?.succeeded) {
        onSubscribed?.(result.data.subscriptionId);
      }
    } catch (error) {
      console.error('Error subscribing:', error);
    }
  };

  const handleCancelSubscription = async (reason: string) => {
    try {
      const result = await cancelSubscription(reason);
      if (result?.succeeded) {
        onCancelled?.();
      }
    } catch (error) {
      console.error('Error canceling subscription:', error);
    }
  };

  const getStatusIcon = () => {
    switch (status) {
      case 'active':
        return <CheckCircle className="h-5 w-5 text-green-500" />;
      case 'trial':
        return <Calendar className="h-5 w-5 text-blue-500" />;
      case 'expired':
      case 'cancelled':
        return <XCircle className="h-5 w-5 text-red-500" />;
      default:
        return <AlertTriangle className="h-5 w-5 text-yellow-500" />;
    }
  };

  const getStatusBadgeVariant = () => {
    switch (status) {
      case 'active':
        return 'success';
      case 'trial':
        return 'info';
      case 'expired':
      case 'cancelled':
        return 'danger';
      default:
        return 'default';
    }
  };

  if (loading && status === null) {
    return (
      <Card>
        <CardContent className="flex items-center justify-center p-6">
          <Loader2 className="h-6 w-6 animate-spin mr-2" />
          <span>Loading subscription status...</span>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="space-y-4">
      {/* Error Display */}
      {error && (
        <Card className="border-red-200 bg-red-50 dark:bg-red-900/10">
          <CardContent className="p-4">
            <div className="flex items-center gap-2 text-red-700 dark:text-red-300">
              <AlertTriangle className="h-4 w-4" />
              <span>{error}</span>
              <Button variant="ghost" size="sm" onClick={clearError} className="ml-auto">
                ✕
              </Button>
            </div>
          </CardContent>
        </Card>
      )}

      {/* Subscription Status Card */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            {getStatusIcon()}
            Subscription Status
          </CardTitle>
          <CardDescription>
            Manage your SocietyLedger subscription
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex items-center justify-between">
            <div className="space-y-1">
              <div className="flex items-center gap-2">
                <Badge variant={getStatusBadgeVariant()}>
                  {status?.toUpperCase()}
                </Badge>
                {status === 'trial' && trialDaysRemaining !== null && (
                  <Badge variant="default">
                    {trialDaysRemaining} days remaining
                  </Badge>
                )}
              </div>
              {planName && (
                <p className="text-sm text-muted-foreground">
                  Plan: {planName}
                  {monthlyAmount && ` - ₹${monthlyAmount}/month`}
                </p>
              )}
              {trialEnd && status === 'trial' && (
                <p className="text-sm text-muted-foreground">
                  Trial ends: {new Date(trialEnd).toLocaleDateString()}
                </p>
              )}
            </div>
            <div className="text-right">
              <div className={`text-lg font-semibold ${accessAllowed ? 'text-green-600' : 'text-red-600'}`}>
                {accessAllowed ? '✓ Access Granted' : '✗ Access Denied'}
              </div>
            </div>
          </div>

          {/* Action Buttons */}
          <div className="flex gap-2 flex-wrap">
            <Button
              variant="outline"
              onClick={refreshStatus}
              disabled={loading}
            >
              {loading ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : null}
              Refresh Status
            </Button>

            {status === 'trial' && (
              <Button
                onClick={() => handleSubscribe('pro-plan', 299, 'razorpay', 'demo_ref')}
                disabled={loading}
              >
                <CreditCard className="h-4 w-4 mr-2" />
                Upgrade to Pro
              </Button>
            )}

            {status === 'active' && (
              <Button
                variant="danger"
                onClick={() => handleCancelSubscription('User requested cancellation')}
                disabled={loading}
              >
                Cancel Subscription
              </Button>
            )}

            {!status && (
              <Button
                onClick={handleCreateTrial}
                disabled={loading}
              >
                Start Free Trial
              </Button>
            )}
          </div>
        </CardContent>
      </Card>

      {/* Usage Instructions */}
      <Card>
        <CardHeader>
          <CardTitle>Integration Notes</CardTitle>
        </CardHeader>
        <CardContent className="text-sm text-muted-foreground space-y-2">
          <p>• Call <code>createTrial()</code> automatically on user registration</p>
          <p>• Check <code>accessAllowed</code> before enabling premium features</p>
          <p>• Use <code>subscribe()</code> after successful payment completion</p>
          <p>• Handle 401 errors by redirecting to login</p>
          <p>• Retry logic is built-in for 500 errors (up to 3 attempts)</p>
        </CardContent>
      </Card>
    </div>
  );
};

// Error Boundary for graceful error handling
interface SubscriptionErrorBoundaryState {
  hasError: boolean;
  error?: Error;
}

export class SubscriptionErrorBoundary extends React.Component<
  React.PropsWithChildren<{}>,
  SubscriptionErrorBoundaryState
> {
  constructor(props: React.PropsWithChildren<{}>) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError(error: Error): SubscriptionErrorBoundaryState {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
    console.error('Subscription component error:', error, errorInfo);
  }

  render() {
    if (this.state.hasError) {
      return (
        <Card>
          <CardContent className="p-6">
            <Alert variant="destructive">
              <AlertTriangle className="h-4 w-4" />
              <AlertDescription>
                Something went wrong with the subscription manager.
                <Button
                  variant="outline"
                  size="sm"
                  className="ml-2"
                  onClick={() => this.setState({ hasError: false })}
                >
                  Try Again
                </Button>
              </AlertDescription>
            </Alert>
          </CardContent>
        </Card>
      );
    }

    return this.props.children;
  }
}