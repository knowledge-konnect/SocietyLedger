"use client"

import { useState } from 'react';
import { CheckCircle, XCircle, AlertTriangle, Calendar, Crown, RefreshCw, Mail, CreditCard, Clock } from 'lucide-react';
import DashboardLayout from '../components/layout/DashboardLayout';
import Button from '../components/ui/Button';
import Card, { CardContent, CardHeader, CardTitle } from '../components/ui/Card';
import Badge from '../components/ui/Badge';
import { Alert, AlertDescription } from '../components/ui/Alert';
import { useSubscription } from '../hooks/useSubscription';
import { useRazorpayPayment } from '../hooks/useRazorpayPayment';
import { useNavigate } from 'react-router-dom';

export default function SubscriptionManagement() {
    const navigate = useNavigate();
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

    const [isRefreshing, setIsRefreshing] = useState(false);
    const [paymentError, setPaymentError] = useState<string | null>(null);

    const { isLoading: isPaymentLoading, isProcessing, initiatePayment } = useRazorpayPayment(
        // onPaymentSuccess
        (subscriptionId: string) => {
            setPaymentError(null);
            // Refresh subscription status to reflect active subscription
            refreshStatus().then(() => {
                // Show success message and redirect or refresh
                alert('Payment successful! Your subscription is now active.');
                navigate('/dashboard');
            });
        },
        // onPaymentError
        (error: string) => {
            setPaymentError(error);
        }
    );

    const handleRefreshStatus = async () => {
        setIsRefreshing(true);
        try {
            await refreshStatus();
        } finally {
            setIsRefreshing(false);
        }
    };

    const handleCreateTrial = async () => {
        try {
            await createTrial();
        } catch (error) {
            console.error('Error creating trial:', error);
        }
    };

    const handleUpgradeSubscription = async () => {
        // Start Razorpay payment flow for upgrade
        const planId = 'pro'; // Or get from context/props
        const amount = monthlyAmount || 299; // Use current plan amount or default

        try {
            await initiatePayment({
                planId,
                amount,
                currency: 'INR',
            });
        } catch (error) {
            setPaymentError(error instanceof Error ? error.message : 'Failed to initiate payment');
        }
    };

    const handleCancelSubscription = async () => {
        const reason = prompt('Please provide a reason for cancellation:');
        if (reason) {
            try {
                await cancelSubscription(reason);
            } catch (error) {
                console.error('Error canceling subscription:', error);
            }
        }
    };

    const getStatusIcon = () => {
        switch (status) {
            case 'active':
                return <CheckCircle className="h-6 w-6 text-green-500" />;
            case 'trial':
                return <Calendar className="h-6 w-6 text-blue-500" />;
            case 'expired':
            case 'cancelled':
                return <XCircle className="h-6 w-6 text-red-500" />;
            default:
                return <AlertTriangle className="h-6 w-6 text-yellow-500" />;
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

    return (
        <DashboardLayout title="Subscription Management">
            <div className="space-y-6">
                {/* Error Display */}
                {error && (
                    <Alert variant="destructive">
                        <AlertTriangle className="h-4 w-4" />
                        <AlertDescription>{error}</AlertDescription>
                        <Button variant="ghost" size="sm" onClick={clearError} className="ml-auto">
                            ✕
                        </Button>
                    </Alert>
                )}

                {/* Payment Error Display */}
                {paymentError && (
                    <Alert variant="destructive">
                        <AlertTriangle className="h-4 w-4" />
                        <AlertDescription>{paymentError}</AlertDescription>
                        <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => setPaymentError(null)}
                            className="ml-auto"
                        >
                            ✕
                        </Button>
                    </Alert>
                )}

                {/* Current Subscription Status */}
                <Card>
                    <CardHeader>
                        <CardTitle className="flex items-center gap-2">
                            {getStatusIcon()}
                            Current Subscription
                        </CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-4">
                        <div className="flex items-center justify-between">
                            <div className="space-y-2">
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
                                <div className="space-y-1">
                                    {planName && (
                                        <p className="text-sm font-medium">
                                            Plan: {planName}
                                        </p>
                                    )}
                                    {monthlyAmount && (
                                        <p className="text-sm text-muted-foreground">
                                            ₹{monthlyAmount}/month
                                        </p>
                                    )}
                                    {trialEnd && status === 'trial' && (
                                        <p className="text-sm text-muted-foreground">
                                            Trial ends: {new Date(trialEnd).toLocaleDateString()}
                                        </p>
                                    )}
                                </div>
                            </div>
                            <div className="text-right">
                                <div className={`text-lg font-semibold ${accessAllowed ? 'text-green-600' : 'text-red-600'}`}>
                                    {accessAllowed ? 'Access Granted' : 'Access Denied'}
                                </div>
                            </div>
                        </div>

                        {/* Action Buttons */}
                        <div className="flex gap-2 flex-wrap">
                            <Button
                                variant="outline"
                                onClick={handleRefreshStatus}
                                disabled={loading || isRefreshing}
                            >
                                <RefreshCw className={`h-4 w-4 mr-2 ${isRefreshing ? 'animate-spin' : ''}`} />
                                Refresh Status
                            </Button>

                            {status === null && (
                                <Button onClick={handleCreateTrial} disabled={loading}>
                                    <Calendar className="h-4 w-4 mr-2" />
                                    Start Free Trial
                                </Button>
                            )}

                            {(status === 'trial' || status === 'expired' || status === 'cancelled') && (
                                <Button
                                    onClick={handleUpgradeSubscription}
                                    disabled={loading || isPaymentLoading || isProcessing}
                                >
                                    <Crown className="h-4 w-4 mr-2" />
                                    {isPaymentLoading || isProcessing ? 'Processing...' : 'Upgrade Now'}
                                </Button>
                            )}

                            {status === 'active' && (
                                <Button variant="danger" onClick={handleCancelSubscription} disabled={loading}>
                                    <XCircle className="h-4 w-4 mr-2" />
                                    Cancel Subscription
                                </Button>
                            )}
                        </div>
                    </CardContent>
                </Card>

                {/* Plan Details */}
                <Card>
                    <CardHeader>
                        <CardTitle>Plan Details</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="space-y-4">
                            <div className="grid md:grid-cols-2 gap-4">
                                <div>
                                    <h4 className="font-medium mb-2">Current Plan</h4>
                                    <p className="text-sm text-muted-foreground">
                                        {planName || 'Free Trial'}
                                    </p>
                                </div>
                                <div>
                                    <h4 className="font-medium mb-2">Monthly Cost</h4>
                                    <p className="text-sm text-muted-foreground">
                                        {monthlyAmount ? `₹${monthlyAmount}` : 'Free'}
                                    </p>
                                </div>
                                <div>
                                    <h4 className="font-medium mb-2">Status</h4>
                                    <p className="text-sm text-muted-foreground capitalize">
                                        {status || 'Not Started'}
                                    </p>
                                </div>
                                <div>
                                    <h4 className="font-medium mb-2">Next Billing</h4>
                                    <p className="text-sm text-muted-foreground">
                                        {status === 'trial' && trialEnd ? new Date(trialEnd).toLocaleDateString() : 'N/A'}
                                    </p>
                                </div>
                            </div>
                        </div>
                    </CardContent>
                </Card>

                {/* Manual Payment Instructions */}
                <Card>
                    <CardHeader>
                        <CardTitle className="flex items-center gap-2">
                            <CreditCard className="h-5 w-5" />
                            Payment Instructions (MVP)
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="space-y-4">
                            <p className="text-sm text-muted-foreground">
                                For the MVP version, all payments are processed manually outside the app.
                            </p>
                            <div className="bg-muted p-4 rounded-lg">
                                <h4 className="font-medium mb-2">To upgrade or renew:</h4>
                                <ol className="list-decimal list-inside space-y-1 text-sm">
                                    <li>Contact our support team at support@societyledger.com</li>
                                    <li>Provide your society details and preferred plan</li>
                                    <li>Make payment via UPI, bank transfer, or cash</li>
                                    <li>Send payment confirmation to support</li>
                                    <li>Your subscription will be activated within 24 hours</li>
                                </ol>
                            </div>
                        </div>
                    </CardContent>
                </Card>

                {/* Support Contact */}
                <Card>
                    <CardHeader>
                        <CardTitle className="flex items-center gap-2">
                            <Mail className="h-5 w-5" />
                            Need Help?
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <p className="text-sm text-muted-foreground mb-4">
                            For subscription questions or support, contact us at:
                        </p>
                        <div className="flex items-center gap-2">
                            <Mail className="h-4 w-4" />
                            <a
                                href="mailto:support@societyledger.com"
                                className="text-primary hover:underline"
                            >
                                support@societyledger.com
                            </a>
                        </div>
                        <p className="text-xs text-muted-foreground mt-2">
                            Response time: Within 24 hours
                        </p>
                    </CardContent>
                </Card>
            </div>
        </DashboardLayout>
    );
}