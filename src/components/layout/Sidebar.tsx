import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { LayoutDashboard, Building2, FileText, CreditCard, TrendingUp, Users, Settings, LogOut, X } from 'lucide-react';
import { useAuth } from '../../contexts/AuthProvider';
import { useToast } from '../ui/Toast';
import { cn } from '../../lib/utils';
import { AlertMessages } from '../../lib/alertMessages';

const navigation = [
  { name: 'Dashboard', href: '/dashboard', icon: LayoutDashboard },
  { name: 'Flats', href: '/flats', icon: Building2 },
  { name: 'Billing', href: '/billing', icon: FileText },
  { name: 'Payments', href: '/payments', icon: CreditCard },
  { name: 'Expenses', href: '/expenses', icon: TrendingUp },
  { name: 'Reports', href: '/reports', icon: TrendingUp },
  { name: 'Users', href: '/users', icon: Users },
  { name: 'Settings', href: '/settings', icon: Settings },
];

export default function Sidebar({ mobileOpen, setMobileOpen }: { mobileOpen?: boolean; setMobileOpen?: (v: boolean) => void } ) {
  const [internalMobileOpen, setInternalMobileOpen] = useState(false);
  const controlled = typeof mobileOpen !== 'undefined' && typeof setMobileOpen === 'function';
  const isMobileOpen = controlled ? mobileOpen : internalMobileOpen;
  const setIsMobileOpen = controlled ? setMobileOpen! : setInternalMobileOpen;
  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const currentPath = window.location.pathname;
  const { logout, user } = useAuth();
  const navigate = useNavigate();
  const { showToast } = useToast();

  return (
    <>
      {isMobileOpen && (
        <div
          className="lg:hidden fixed inset-0 z-40 bg-black/40 animate-fade-in"
          onClick={() => setIsMobileOpen(false)}
        />
      )}

      <aside
        id="sidebar"
        className={cn(
          'fixed top-0 left-0 bottom-0 w-64 bg-white dark:bg-slate-900 border-r border-slate-200 dark:border-slate-700 z-50 transform transition-smooth duration-500',
          isMobileOpen ? 'translate-x-0' : '-translate-x-full'
        )}
        aria-hidden={!isMobileOpen}
      >
        <div className="h-full flex flex-col">
          {/* Header with logo and brand */}
          <div className="p-5 border-b border-slate-200 dark:border-slate-700 flex items-center justify-between">
            <div className="flex items-center gap-3 min-w-0">
              <div className="w-9 h-9 rounded-lg gradient-primary flex items-center justify-center text-white font-bold text-xs flex-shrink-0">
                {((name: string) => {
                  const words = name.trim().split(/\s+/);
                  return words.map(w => w[0]).slice(0, 2).join('').toUpperCase();
                })(user?.societyName || 'SL')}
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-sm font-semibold text-slate-900 dark:text-white line-clamp-2">
                  {user?.societyName || 'SocietyLedger'}
                </p>
                <p className="text-xs text-slate-500 dark:text-slate-400">
                  Menu
                </p>
              </div>
            </div>
            <button
              onClick={() => setIsMobileOpen(false)}
              className="lg:hidden p-1.5 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 transition-smooth duration-200"
              aria-label="Close menu"
            >
              <X className="w-5 h-5 text-slate-600 dark:text-slate-400" />
            </button>
          </div>

          {/* Navigation menu */}
          <nav className="flex-1 p-3 space-y-1 overflow-y-auto">
            {navigation.map((item) => {
              const isActive = currentPath === item.href;
              return (
                <a
                  key={item.name}
                  href={item.href}
                  className={cn(
                    'flex items-center gap-3 px-4 py-2.5 rounded-lg text-sm font-medium transition-smooth duration-200',
                    isActive
                      ? 'bg-indigo-600 text-white shadow-md'
                      : 'text-slate-600 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800'
                  )}
                >
                  <item.icon className="w-5 h-5 flex-shrink-0" />
                  <span>{item.name}</span>
                </a>
              );
            })}
          </nav>

          {/* Logout button */}
          <div className="p-3 border-t border-slate-200 dark:border-slate-700">
            <button
              onClick={async () => {
                setIsLoggingOut(true);
                try {
                  await logout();
                  // Logout clears auth state, which will trigger Router to redirect
                  showToast(AlertMessages.success.logoutSuccess, 'success');
                  // Small delay to ensure state is updated before redirect
                  setTimeout(() => {
                    navigate('/login');
                  }, 100);
                } catch (error: any) {
                  showToast(AlertMessages.error.logoutFailed, 'error');
                  setIsLoggingOut(false);
                }
              }}
              disabled={isLoggingOut}
              className="flex items-center gap-3 px-4 py-2.5 rounded-lg text-sm font-medium text-destructive hover:bg-destructive/10 w-full transition-smooth duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <LogOut className="w-5 h-5 flex-shrink-0" />
              {isLoggingOut ? 'Logging out...' : 'Logout'}
            </button>
          </div>
        </div>
      </aside>
    </>
  );
}
