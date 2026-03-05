import React from 'react';
import { useNavigate } from 'react-router-dom';
import Badge from '../components/ui/Badge';
import Button from '../components/ui/Button';
import Card, { CardContent, CardHeader, CardTitle } from '../components/ui/Card';
import { CheckCircle, Calendar, XCircle, AlertTriangle, Crown } from 'lucide-react';
import { useSubscription } from '../hooks/useSubscription';

export const SubscriptionSummary: React.FC = () => {
  const navigate = useNavigate();
  const { status, trialDaysRemaining, planName, monthlyAmount, accessAllowed, loading } = useSubscription();

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

  const getActionButton = () => {
    if (status === 'trial' || status === 'expired' || status === 'cancelled') {
      return (
        <Button size="sm" onClick={() => navigate('/subscription/manage')}>
          <Crown className="h-4 w-4 mr-2" />
          Upgrade
        </Button>
      );
    }
    return (
      <Button variant="outline" size="sm" onClick={() => navigate('/subscription/manage')}>
        View Details
      </Button>
    );
  };

  if (loading) {
    return (
      <Card>
        <CardContent className="flex items-center justify-center p-4">
          <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-primary mr-2"></div>
          <span className="text-sm">Loading...</span>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader className="pb-3">
        <CardTitle className="flex items-center gap-2 text-lg">
          {getStatusIcon()}
          Subscription
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-3">
        <div className="flex items-center justify-between">
          <div className="space-y-1">
            <div className="flex items-center gap-2">
              <Badge variant={getStatusBadgeVariant()}>
                {status?.toUpperCase()}
              </Badge>
              {status === 'trial' && trialDaysRemaining !== null && (
                <Badge variant="default">
                  {trialDaysRemaining} days left
                </Badge>
              )}
            </div>
            {planName && (
              <p className="text-sm text-muted-foreground">
                {planName}
                {monthlyAmount && ` - ₹${monthlyAmount}/month`}
              </p>
            )}
          </div>
          {getActionButton()}
        </div>
      </CardContent>
    </Card>
  );
};