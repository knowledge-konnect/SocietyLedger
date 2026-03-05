import { useState } from 'react';
import { Plus, X, FileText, CreditCard, TrendingDown, Building } from 'lucide-react';
import { cn } from '../../lib/utils';

interface FABAction {
  icon: React.ElementType;
  label: string;
  onClick: () => void;
  color: string;
}

export default function MobileFAB() {
  const [isOpen, setIsOpen] = useState(false);

  const actions: FABAction[] = [
    {
      icon: FileText,
      label: 'Generate Bills',
      onClick: () => window.location.href = '/billing',
      color: 'bg-indigo-600',
    },
    {
      icon: CreditCard,
      label: 'Record Payment',
      onClick: () => window.location.href = '/payments',
      color: 'bg-green-600',
    },
    {
      icon: TrendingDown,
      label: 'Add Expense',
      onClick: () => window.location.href = '/expenses',
      color: 'bg-red-600',
    },
    {
      icon: Building,
      label: 'Add Flat',
      onClick: () => window.location.href = '/flats',
      color: 'bg-violet-600',
    },
  ];

  return (
    <>
      <div className="lg:hidden fixed bottom-6 right-6 z-50">
        <div className={cn('flex flex-col-reverse gap-3 mb-3', isOpen ? 'block' : 'hidden')}>
          {actions.map((action, index) => {
            const Icon = action.icon;
            return (
              <div
                key={index}
                className="flex items-center gap-3 animate-slide-up"
                style={{ animationDelay: `${index * 50}ms` }}
              >
                <span className="bg-slate-900 dark:bg-slate-100 text-white dark:text-slate-900 px-3 py-2 rounded-lg text-sm font-medium whitespace-nowrap shadow-lg">
                  {action.label}
                </span>
                <button
                  onClick={() => {
                    action.onClick();
                    setIsOpen(false);
                  }}
                  className={cn(
                    'w-12 h-12 rounded-full shadow-premium-xl flex items-center justify-center text-white transition-transform hover:scale-110',
                    action.color
                  )}
                >
                  <Icon className="w-5 h-5" />
                </button>
              </div>
            );
          })}
        </div>

        <button
          onClick={() => setIsOpen(!isOpen)}
          className={cn(
            'w-14 h-14 rounded-full shadow-premium-xl flex items-center justify-center text-white transition-all duration-300',
            isOpen
              ? 'bg-red-600 rotate-45'
              : 'bg-gradient-to-r from-indigo-600 to-indigo-500'
          )}
        >
          {isOpen ? <X className="w-6 h-6" /> : <Plus className="w-6 h-6" />}
        </button>
      </div>

      {isOpen && (
        <div
          className="lg:hidden fixed inset-0 bg-black/20 z-40 animate-fade-in"
          onClick={() => setIsOpen(false)}
        />
      )}
    </>
  );
}
