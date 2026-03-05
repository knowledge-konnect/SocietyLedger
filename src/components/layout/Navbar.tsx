import React from 'react';
import { Building2, Moon, Sun } from 'lucide-react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useTheme } from '../../contexts/ThemeContext';
import Button from '../ui/Button';

interface NavbarProps {
  variant?: 'landing' | 'dashboard';
}

export default function Navbar({ variant = 'landing' }: NavbarProps) {
  const { theme, toggleTheme } = useTheme();
  const navigate = useNavigate();
  const location = useLocation();

  const scrollToSection = (id: string) => (e?: React.MouseEvent) => {
    e?.preventDefault();
    const scrollWithOffset = () => {
      const el = document.getElementById(id);
      if (el) {
        const offsetTop = el.offsetTop - 200;
        window.scrollTo({ top: offsetTop, behavior: 'smooth' });
      }
    };
    if (location.pathname === '/') {
      scrollWithOffset();
      return;
    }
    navigate('/');
    setTimeout(scrollWithOffset, 120);
  };

  if (variant === 'landing') {
    return (
      <nav className="fixed top-0 left-0 right-0 z-50 bg-white/80 dark:bg-slate-950/80 backdrop-blur-xl border-b border-slate-200 dark:border-slate-800 shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex items-center justify-between h-16">
            <div className="flex items-center gap-3">
              <Link to="/" className="flex items-center gap-2.5 group">
                <div className="relative w-11 h-11 rounded-lg bg-gradient-to-br from-indigo-600 to-indigo-700 dark:from-indigo-500 dark:to-indigo-600 group-hover:shadow-lg transition-all duration-300 flex items-center justify-center shadow-md hover:-translate-y-0.5">
                  <Building2 className="w-6 h-6 text-white" />
                </div>
                <div className="flex flex-col">
                  <span className="text-lg font-bold text-slate-900 dark:text-white group-hover:text-indigo-600 dark:group-hover:text-indigo-400 transition-colors duration-300">SocietyLedger</span>
                  <span className="text-xs text-slate-500 dark:text-slate-400 font-medium">Smart Management</span>
                </div>
              </Link>
            </div>

            <div className="hidden md:flex items-center gap-1">
              <a onClick={scrollToSection('features')} href="#features" className="px-4 py-2 text-slate-700 dark:text-slate-300 hover:text-indigo-600 dark:hover:text-indigo-400 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition-all duration-300 font-medium text-sm">
                Features
              </a>
              <a onClick={scrollToSection('pricing')} href="#pricing" className="px-4 py-2 text-slate-700 dark:text-slate-300 hover:text-indigo-600 dark:hover:text-indigo-400 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition-all duration-300 font-medium text-sm">
                Pricing
              </a>
              <a onClick={scrollToSection('faq')} href="#faq" className="px-4 py-2 text-slate-700 dark:text-slate-300 hover:text-indigo-600 dark:hover:text-indigo-400 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition-all duration-300 font-medium text-sm">
                FAQ
              </a>
            </div>

            <div className="flex items-center gap-3">
              <button
                onClick={toggleTheme}
                className="p-2 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors duration-300"
                aria-label="Toggle theme"
              >
                {theme === 'light' ? (
                  <Moon className="w-5 h-5 text-slate-600 hover:text-primary transition-colors" />
                ) : (
                  <Sun className="w-5 h-5 text-slate-300 hover:text-primary transition-colors" />
                )}
              </button>
              <Link to="/login">
                <Button variant="outline" size="sm">Login</Button>
              </Link>
              <Link to="/signup">
                <Button size="sm">Start Free</Button>
              </Link>
            </div>
          </div>
        </div>
      </nav>
    );
  }

  return null;
}
