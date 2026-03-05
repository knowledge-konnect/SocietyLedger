import { Building2, Mail, Lock } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import { useAuth } from '../contexts/AuthProvider';
import { useToast } from '../components/ui/Toast';
import { handleApiError } from '../api/client';
import { AlertMessages } from '../lib/alertMessages';

const loginSchema = z.object({
  usernameOrEmail: z.string().min(1, 'Email or username is required'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
});

type LoginFormData = z.infer<typeof loginSchema>;

export default function Login() {
  const { showToast } = useToast();
  const { login } = useAuth();
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    setIsLoading(true);
    try {
      await login(data);
      showToast(AlertMessages.success.loginSuccess, 'success');
      
      // Check if user needs to change password - Router will handle the redirect
      // Router checks the forcePasswordChange flag automatically
      // So just navigate to dashboard and Router will redirect if needed
      setTimeout(() => {
        navigate('/dashboard');
      }, 500);
    } catch (error: any) {
      showToast(AlertMessages.error.invalidCredentials, 'error');
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-background flex flex-col items-center justify-center px-4 py-12">
      <div className="w-full max-w-md">
        {/* Brand Header */}
        <div className="text-center mb-10">
          <div className="inline-flex items-center justify-center w-14 h-14 gradient-primary rounded-xl mb-4">
            <Building2 className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-3xl font-bold text-foreground mb-2">Welcome back</h1>
          <p className="subheading">Sign in to your society dashboard</p>
        </div>

        {/* Form Card */}
        <div className="card-base p-8 shadow-md">
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
            <Input
              label="Email or Username"
              placeholder="your@email.com or username"
              icon={<Mail className="w-5 h-5" />}
              error={errors.usernameOrEmail?.message}
              {...register('usernameOrEmail')}
            />

            <Input
              label="Password"
              type="password"
              placeholder="Enter your password"
              icon={<Lock className="w-5 h-5" />}
              error={errors.password?.message}
              {...register('password')}
            />

            <div className="flex items-center justify-between text-sm pt-2">
              <label className="flex items-center gap-2 text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-slate-300 cursor-pointer transition-colors">
                <input
                  type="checkbox"
                  className="rounded border-slate-300 dark:border-slate-600 accent-indigo-600"
                />
                Remember me
              </label>
              <a
                href="/forgot-password"
                className="text-indigo-600 dark:text-indigo-400 hover:text-indigo-500 dark:hover:text-indigo-300 font-semibold micro-interaction"
              >
                Forgot password?
              </a>
            </div>

            <Button
              type="submit"
              variant="primary"
              size="lg"
              className="w-full mt-6"
              isLoading={isLoading}
            >
              Sign In
            </Button>
          </form>

          <div className="mt-6 text-center">
            <p className="text-sm text-slate-600 dark:text-slate-400">
              Don't have an account?{' '}
              <a
                href="/signup"
                className="text-indigo-600 dark:text-indigo-400 hover:text-indigo-500 dark:hover:text-indigo-300 font-semibold micro-interaction"
              >
                Sign up
              </a>
            </p>
          </div>
        </div>

        {/* Footer */}
        <p className="mt-6 text-center text-xs text-slate-600 dark:text-slate-400">
          By signing in, you agree to our{' '}
          <a href="#" className="text-indigo-600 dark:text-indigo-400 hover:text-indigo-500 dark:hover:text-indigo-300 font-semibold">
            Terms of Service
          </a>{' '}
          and{' '}
          <a href="#" className="text-indigo-600 dark:text-indigo-400 hover:text-indigo-500 dark:hover:text-indigo-300 font-semibold">
            Privacy Policy
          </a>
        </p>
      </div>
    </div>
  );
}
