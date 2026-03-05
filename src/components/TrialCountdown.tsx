import { useState, useEffect } from 'react'
import { Clock, AlertTriangle } from 'lucide-react'

interface TrialCountdownProps {
  trialDaysRemaining: number
  status: 'trial' | 'active' | 'expired' | 'past_due' | 'cancelled'
  className?: string
  showIcon?: boolean
  compact?: boolean
}

export default function TrialCountdown({
  trialDaysRemaining,
  status,
  className = '',
  showIcon = true,
  compact = false
}: TrialCountdownProps) {
  const [timeLeft, setTimeLeft] = useState({
    days: trialDaysRemaining,
    hours: 0,
    minutes: 0,
    seconds: 0
  })

  useEffect(() => {
    if (status !== 'trial' || trialDaysRemaining <= 0) return

    const timer = setInterval(() => {
      const now = new Date()
      const trialEnd = new Date(now.getTime() + trialDaysRemaining * 24 * 60 * 60 * 1000)
      const difference = trialEnd.getTime() - now.getTime()

      if (difference > 0) {
        const days = Math.floor(difference / (1000 * 60 * 60 * 24))
        const hours = Math.floor((difference % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
        const minutes = Math.floor((difference % (1000 * 60 * 60)) / (1000 * 60))
        const seconds = Math.floor((difference % (1000 * 60)) / 1000)

        setTimeLeft({ days, hours, minutes, seconds })
      }
    }, 1000)

    return () => clearInterval(timer)
  }, [trialDaysRemaining, status])

  if (status === 'active') {
    return null // Don't show countdown for active subscriptions
  }

  if (status === 'expired' || status === 'past_due') {
    return (
      <div className={`flex items-center gap-2 p-3 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg ${className}`}>
        {showIcon && <AlertTriangle className="w-4 h-4 text-red-600 dark:text-red-400 flex-shrink-0" />}
        <div className="text-sm">
          <span className="font-semibold text-red-800 dark:text-red-200">
            {status === 'expired' ? 'Trial Expired' : 'Payment Overdue'}
          </span>
          <span className="text-red-600 dark:text-red-400 block">
            {status === 'expired'
              ? 'Subscribe now to continue using SocietyLedger'
              : 'Please update your payment method'
            }
          </span>
        </div>
      </div>
    )
  }

  if (trialDaysRemaining <= 0) {
    return (
      <div className={`flex items-center gap-2 p-3 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg ${className}`}>
        {showIcon && <AlertTriangle className="w-4 h-4 text-red-600 dark:text-red-400 flex-shrink-0" />}
        <div className="text-sm">
          <span className="font-semibold text-red-800 dark:text-red-200">Trial Expired</span>
          <span className="text-red-600 dark:text-red-400 block">Subscribe now to continue using SocietyLedger</span>
        </div>
      </div>
    )
  }

  const isUrgent = trialDaysRemaining <= 7
  const bgColor = isUrgent
    ? 'bg-amber-50 dark:bg-amber-900/20 border-amber-200 dark:border-amber-800'
    : 'bg-blue-50 dark:bg-blue-900/20 border-blue-200 dark:border-blue-800'
  const textColor = isUrgent
    ? 'text-amber-800 dark:text-amber-200'
    : 'text-blue-800 dark:text-blue-200'
  const accentColor = isUrgent
    ? 'text-amber-600 dark:text-amber-400'
    : 'text-blue-600 dark:text-blue-400'

  if (compact) {
    return (
      <div className={`flex items-center gap-2 p-2 ${bgColor} border rounded-lg ${className}`}>
        {showIcon && <Clock className="w-4 h-4 text-blue-600 dark:text-blue-400 flex-shrink-0" />}
        <span className={`text-sm font-medium ${textColor}`}>
          {trialDaysRemaining} day{trialDaysRemaining !== 1 ? 's' : ''} left
        </span>
      </div>
    )
  }

  return (
    <div className={`p-4 ${bgColor} border rounded-lg ${className}`}>
      <div className="flex items-center gap-3 mb-2">
        {showIcon && <Clock className="w-5 h-5 text-blue-600 dark:text-blue-400 flex-shrink-0" />}
        <div>
          <h3 className={`font-semibold ${textColor}`}>
            {isUrgent ? 'Trial Ending Soon' : 'Free Trial Active'}
          </h3>
          <p className={`text-sm ${accentColor}`}>
            {trialDaysRemaining} day{trialDaysRemaining !== 1 ? 's' : ''} remaining
          </p>
        </div>
      </div>

      {isUrgent && (
        <div className="grid grid-cols-4 gap-2 mt-3">
          <div className="text-center">
            <div className={`text-lg font-bold ${textColor}`}>{timeLeft.days}</div>
            <div className={`text-xs ${accentColor}`}>Days</div>
          </div>
          <div className="text-center">
            <div className={`text-lg font-bold ${textColor}`}>{timeLeft.hours}</div>
            <div className={`text-xs ${accentColor}`}>Hours</div>
          </div>
          <div className="text-center">
            <div className={`text-lg font-bold ${textColor}`}>{timeLeft.minutes}</div>
            <div className={`text-xs ${accentColor}`}>Min</div>
          </div>
          <div className="text-center">
            <div className={`text-lg font-bold ${textColor}`}>{timeLeft.seconds}</div>
            <div className={`text-xs ${accentColor}`}>Sec</div>
          </div>
        </div>
      )}
    </div>
  )
}