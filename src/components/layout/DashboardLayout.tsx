import { ReactNode, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Moon, Sun, Bell, LogOut, UserIcon, ChevronLeft } from 'lucide-react';
import Sidebar from './Sidebar';
import { useTheme } from '../../contexts/ThemeContext';
import { useAuth } from '../../contexts/AuthProvider';
import { useToast } from '../ui/Toast';
import { cn } from '../../lib/utils';
import { AlertMessages } from '../../lib/alertMessages';

interface DashboardLayoutProps {
  children: ReactNode;
  title?: string;
}

export default function DashboardLayout({ children, title }: DashboardLayoutProps) {
  const { theme, toggleTheme } = useTheme();
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const { showToast } = useToast();
  const [isProfileMenuOpen, setIsProfileMenuOpen] = useState(false);
  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const [isSidebarOpen, setIsSidebarOpen] = useState<boolean>(() => {
    try {
      return typeof window !== 'undefined' ? window.innerWidth >= 1024 : false;
    } catch (e) {
      return false;
    }
  });

  const handleLogout = async () => {
    setIsLoggingOut(true);
    try {
      await logout();
      showToast(AlertMessages.success.logoutSuccess, 'success');
      navigate('/login');
    } catch (error: any) {
      showToast(AlertMessages.error.logoutFailed, 'error');
      setIsLoggingOut(false);
    }
  };

  return (
    <div className="min-h-screen bg-slate-50 dark:bg-slate-950">
      <Sidebar mobileOpen={isSidebarOpen} setMobileOpen={setIsSidebarOpen} />

      <div className={`transition-smooth duration-500 ${isSidebarOpen ? 'lg:pl-64' : 'lg:pl-0'}`}>
        <header className="sticky top-0 z-30 bg-white dark:bg-slate-900 border-b border-slate-200 dark:border-slate-700 px-4 lg:px-6 py-3 lg:py-4 backdrop-blur-sm bg-opacity-98 dark:bg-opacity-98">
          <div className="flex items-center justify-between gap-4">
            <div className="flex items-center gap-4 flex-1 min-w-0">
              <button
                onClick={() => setIsSidebarOpen(!isSidebarOpen)}
                aria-label="Toggle menu"
                aria-expanded={isSidebarOpen}
                aria-controls="sidebar"
                className="p-2 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 transition-smooth duration-200 flex-shrink-0"
              >
                <ChevronLeft className={`w-5 h-5 text-slate-600 dark:text-slate-400 transition-transform duration-300 ${isSidebarOpen ? 'rotate-180' : 'rotate-0'}`} />
              </button>

              <div className="hidden md:block flex-1 min-w-0">
                {title && (
                  <h1 className="text-base lg:text-lg font-semibold text-slate-900 dark:text-white truncate">
                    {title}
                  </h1>
                )}
              </div>
            </div>

            <div className="flex items-center gap-2 lg:gap-3 flex-shrink-0">
              <button
                onClick={toggleTheme}
                className="p-2 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 transition-smooth duration-200"
                aria-label="Toggle theme"
              >
                {theme === 'light' ? (
                  <Moon className="w-5 h-5 text-slate-600 dark:text-slate-400 transition-colors" />
                ) : (
                  <Sun className="w-5 h-5 text-slate-400 dark:text-slate-300 transition-colors" />
                )}
              </button>

              <button className="relative p-2 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 transition-smooth duration-200" aria-label="Notifications">
                <Bell className="w-5 h-5 text-slate-600 dark:text-slate-400 transition-colors" />
                <span className="absolute top-1.5 right-1.5 w-2 h-2 bg-red-500 rounded-full animate-pulse-soft"></span>
              </button>

              <div className="flex items-center gap-2 pl-2 lg:pl-3 hidden sm:flex border-l border-slate-200 dark:border-slate-700">
                <div className="relative">
                  <button
                    onClick={() => setIsProfileMenuOpen(!isProfileMenuOpen)}
                    className="flex items-center gap-2 p-1.5 lg:p-2 rounded-lg transition-smooth duration-200 focus:outline-none focus:ring-2 focus:ring-indigo-500/50 focus:ring-offset-2 dark:focus:ring-offset-slate-900"
                    aria-label="Profile menu"
                    aria-expanded={isProfileMenuOpen}
                  >
                    <div className="w-8 h-8 bg-gradient-to-br from-indigo-600 to-indigo-500 rounded-lg flex items-center justify-center text-white font-bold text-xs lg:text-sm shadow-md">
                      {user?.name
                        ? user.name
                          .split(' ')
                          .map(word => word[0])
                          .join('')
                          .substring(0, 2)
                          .toUpperCase()
                        : 'UN'}
                    </div>
                    <div className="hidden lg:block text-left">
                      <p className="text-sm font-medium text-slate-900 dark:text-white leading-tight">
                        {user?.name
                          ? user.name
                            .split(' ')
                            .slice(0, 2)
                            .join(' ')
                            .substring(0, 20)
                          : 'User'}
                      </p>
                      <p className="text-xs text-slate-500 dark:text-slate-400">
                        {user?.role || user?.roles?.[0] || 'Admin'}
                      </p>
                    </div>
                  </button>

                  {isProfileMenuOpen && (
                    <>
                      <div
                        className="fixed inset-0 z-30 animate-fade-in"
                        onClick={() => setIsProfileMenuOpen(false)}
                        aria-hidden="true"
                      />
                      <div className="absolute top-full right-0 mt-2 w-56 bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-lg shadow-md z-40 overflow-hidden animate-scale-in">
                        <div className="p-4 border-b border-slate-200 dark:border-slate-700">
                          <p className="text-sm font-medium text-slate-900 dark:text-white">
                            {user?.name || 'User'}
                          </p>
                          <p className="text-xs text-slate-500 dark:text-slate-400 mt-1">
                            {user?.role || user?.roles?.[0] || 'Admin'}
                          </p>
                        </div>
                        <button
                          onClick={() => {
                            setIsProfileMenuOpen(false);
                            navigate('/settings');
                          }}
                          className="flex items-center gap-3 w-full px-4 py-2.5 text-sm text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-800 transition-smooth focus:outline-none"
                        >
                          <UserIcon className="w-4 h-4" />
                          Settings
                        </button>
                        <button
                          onClick={() => {
                            setIsProfileMenuOpen(false);
                            handleLogout();
                          }}
                          disabled={isLoggingOut}
                          className="flex items-center gap-3 w-full px-4 py-2.5 text-sm text-red-600 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20 border-t border-slate-200 dark:border-slate-700 transition-smooth disabled:opacity-50 focus:outline-none"
                        >
                          <LogOut className="w-4 h-4" />
                          {isLoggingOut ? 'Logging out...' : 'Logout'}
                        </button>
                      </div>
                    </>
                  )}
                </div>
              </div>
            </div>
          </div>
        </header>

        <main className="py-4 lg:py-6">
          <div className="app-container">
            <div className="page-panel space-y-6">
              {children}
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}
