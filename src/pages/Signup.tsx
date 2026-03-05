import { Building2, User, Mail, Lock, MapPin } from 'lucide-react';
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

const signupSchema = z.object({
  name: z.string().min(2, 'Name must be at least 2 characters'),
  email: z.string().email('Invalid email address'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
  societyName: z.string().min(2, 'Society name is required'),
  societyAddress: z.string().min(5, 'Please enter a valid address'),
});

type SignupFormData = z.infer<typeof signupSchema>;

export default function Signup() {
  const { showToast } = useToast();
  const { register: registerUser } = useAuth();
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<SignupFormData>({
    resolver: zodResolver(signupSchema),
  });

  const onSubmit = async (data: SignupFormData) => {
    setIsLoading(true);
    try {
      await registerUser({
        name: data.name,
        email: data.email,
        password: data.password,
        societyName: data.societyName,
        societyAddress: data.societyAddress,
      });
      showToast(AlertMessages.success.signupSuccess, 'success');
      navigate('/dashboard');
    } catch (error: any) {
      const apiError = handleApiError(error);
      showToast(AlertMessages.error.signupFailed, 'error');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-background flex flex-col items-center justify-center px-4 py-12">
      <div className="w-full max-w-2xl">
        {/* Brand Header */}
        <div className="text-center mb-10">
          <div className="inline-flex items-center justify-center w-14 h-14 gradient-primary rounded-xl mb-4">
            <Building2 className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-3xl font-bold text-foreground mb-2">Create your account</h1>
          <p className="subheading">Set up your society management in minutes</p>
        </div>

        {/* Form Card */}
        <div className="card-base p-8 shadow-md">
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            {/* Account Details Section */}
            <div>
              <h3 className="text-sm font-semibold text-foreground mb-4">Account Details</h3>
              <div className="grid md:grid-cols-2 gap-5">
                <Input
                  label="Full Name"
                  placeholder="John Doe"
                  icon={<User className="w-5 h-5" />}
                  error={errors.name?.message}
                  {...register('name')}
                />

                <Input
                  label="Email Address"
                  type="email"
                  placeholder="your@email.com"
                  icon={<Mail className="w-5 h-5" />}
                  error={errors.email?.message}
                  {...register('email')}
                />
              </div>

              <div className="mt-5">
                <Input
                  label="Password"
                  type="password"
                  placeholder="Min. 6 characters"
                  icon={<Lock className="w-5 h-5" />}
                  error={errors.password?.message}
                  {...register('password')}
                />
              </div>
            </div>

            {/* Divider */}
            <div className="divider" />

            {/* Society Details Section */}
            <div>
              <h3 className="text-sm font-semibold text-foreground mb-4">Society Information</h3>
              <div className="space-y-5">
                <Input
                  label="Society Name"
                  placeholder="e.g., Greenwoods Apartments"
                  icon={<Building2 className="w-5 h-5" />}
                  error={errors.societyName?.message}
                  {...register('societyName')}
                />

                <div>
                  <label className="block text-sm font-semibold text-foreground mb-2">
                    Society Address
                  </label>
                  <div className="relative">
                    <MapPin className="absolute left-3 top-3 w-5 h-5 text-slate-400 dark:text-slate-500" />
                    <textarea
                      placeholder="Enter full society address"
                      rows={3}
                      className={`input-base pl-10 resize-none ${errors.societyAddress ? 'border-destructive focus:ring-destructive/50' : ''}`}
                      {...register('societyAddress')}
                    />
                  </div>
                  {errors.societyAddress && (
                    <p className="mt-2 text-sm text-destructive font-medium">{errors.societyAddress.message}</p>
                  )}
                </div>
              </div>
            </div>

            <Button
              type="submit"
              variant="primary"
              size="lg"
              className="w-full mt-8"
              isLoading={isLoading}
            >
              Create Account
            </Button>
          </form>

          <div className="mt-6 text-center">
            <p className="text-sm text-slate-600 dark:text-slate-400">
              Already have an account?{' '}
              <a
                href="/login"
                className="text-indigo-600 dark:text-indigo-400 hover:text-indigo-500 dark:hover:text-indigo-300 font-semibold micro-interaction"
              >
                Sign in
              </a>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
