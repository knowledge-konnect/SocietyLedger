import { useState } from 'react';
import { Plus, Zap, Droplet, Shield, Wrench, Users as UsersIcon, MoreHorizontal, Calendar, TrendingDown } from 'lucide-react';
import { PieChart, Pie, Cell, ResponsiveContainer, Legend, Tooltip } from 'recharts';
import DashboardLayout from '../components/layout/DashboardLayout';
import Card, { CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import Select from '../components/ui/Select';
import Modal, { ModalFooter } from '../components/ui/Modal';
import { Table, TableHeader, TableBody, TableRow, TableHead, TableCell } from '../components/ui/Table';
import Badge from '../components/ui/Badge';
import { formatCurrency, formatDate } from '../lib/utils';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';

const expenseSchema = z.object({
  category: z.string().min(1, 'Category is required'),
  vendor: z.string().min(1, 'Vendor name is required'),
  amount: z.string().min(1, 'Amount is required'),
  expenseDate: z.string().min(1, 'Date is required'),
  notes: z.string().optional(),
});

type ExpenseFormData = z.infer<typeof expenseSchema>;

const mockExpenses = [
  { id: '1', category: 'electricity', vendor: 'City Power Co.', amount: 12500, expenseDate: '2025-01-05', notes: 'Monthly electricity bill' },
  { id: '2', category: 'water', vendor: 'Municipal Water', amount: 3500, expenseDate: '2025-01-08', notes: 'Water charges' },
  { id: '3', category: 'security', vendor: 'SecureGuard Services', amount: 45000, expenseDate: '2025-01-10', notes: 'Security staff salaries' },
  { id: '4', category: 'repairs', vendor: 'Fix-It Services', amount: 8500, expenseDate: '2025-01-12', notes: 'Lift maintenance' },
  { id: '5', category: 'salary', vendor: 'Housekeeping Staff', amount: 35000, expenseDate: '2025-01-15', notes: 'Monthly salaries' },
  { id: '6', category: 'others', vendor: 'Garden Supplies', amount: 4200, expenseDate: '2025-01-18', notes: 'Plants and fertilizers' },
];

const categoryConfig = {
  electricity: { label: 'Electricity', icon: Zap, color: '#eab308' },
  water: { label: 'Water', icon: Droplet, color: '#3b82f6' },
  security: { label: 'Security', icon: Shield, color: '#8b5cf6' },
  repairs: { label: 'Repairs', icon: Wrench, color: '#f97316' },
  salary: { label: 'Salary', icon: UsersIcon, color: '#10b981' },
  others: { label: 'Others', icon: MoreHorizontal, color: '#6b7280' },
};

export default function Expenses() {
  const [showAddModal, setShowAddModal] = useState(false);
  const [selectedMonth, setSelectedMonth] = useState('2025-01');

  const { register, handleSubmit, formState: { errors }, reset } = useForm<ExpenseFormData>({
    resolver: zodResolver(expenseSchema),
  });

  const onSubmit = (data: ExpenseFormData) => {
    console.log('Expense data:', data);
    setShowAddModal(false);
    reset();
  };

  const totalExpenses = mockExpenses.reduce((sum, e) => sum + e.amount, 0);

  const categoryData = Object.entries(
    mockExpenses.reduce((acc, expense) => {
      acc[expense.category] = (acc[expense.category] || 0) + expense.amount;
      return acc;
    }, {} as Record<string, number>)
  ).map(([category, value]) => ({
    name: categoryConfig[category as keyof typeof categoryConfig].label,
    value,
    color: categoryConfig[category as keyof typeof categoryConfig].color,
  }));

  return (
    <DashboardLayout title="Expenses">
      <div className="space-y-6">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <Card className="lg:col-span-2">
            <CardHeader>
              <div className="flex items-center justify-between">
                <CardTitle>Expense Breakdown</CardTitle>
                <Select
                  options={[
                    { value: '2025-01', label: 'January 2025' },
                    { value: '2024-12', label: 'December 2024' },
                  ]}
                  value={selectedMonth}
                  onChange={(e) => setSelectedMonth(e.target.value)}
                />
              </div>
            </CardHeader>
            <CardContent>
              <ResponsiveContainer width="100%" height={300}>
                <PieChart>
                  <Pie
                    data={categoryData}
                    cx="50%"
                    cy="50%"
                    labelLine={false}
                    label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                    outerRadius={100}
                    fill="#8884d8"
                    dataKey="value"
                  >
                    {categoryData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={entry.color} />
                    ))}
                  </Pie>
                  <Tooltip formatter={(value: number) => formatCurrency(value)} />
                  <Legend />
                </PieChart>
              </ResponsiveContainer>
            </CardContent>
          </Card>

          <div className="space-y-4">
            <Card className="bg-gradient-to-br from-red-50 to-orange-100 dark:from-red-900/20 dark:to-orange-800/20 border-red-200 dark:border-red-800">
              <CardContent className="p-6">
                <div className="w-14 h-14 bg-red-500 rounded-2xl flex items-center justify-center shadow-lg mb-4">
                  <TrendingDown className="w-7 h-7 text-white" />
                </div>
                <p className="text-sm text-red-700 dark:text-red-300 font-medium mb-2">Total Expenses (MTD)</p>
                <p className="text-3xl font-bold text-red-900 dark:text-red-100">
                  {formatCurrency(totalExpenses)}
                </p>
                <p className="text-xs text-red-600 dark:text-red-400 mt-2">
                  {mockExpenses.length} transactions
                </p>
              </CardContent>
            </Card>

            <Button className="w-full" size="lg" onClick={() => setShowAddModal(true)}>
              <Plus className="w-5 h-5 mr-2" />
              Add Expense
            </Button>

            <Card>
              <CardContent className="p-4">
                <h4 className="font-semibold text-gray-900 dark:text-white mb-3 text-sm">Categories</h4>
                <div className="space-y-2">
                  {Object.entries(categoryConfig).map(([key, config]) => {
                    const Icon = config.icon;
                    const amount = categoryData.find(d => d.name === config.label)?.value || 0;
                    return (
                      <div key={key} className="flex items-center justify-between p-2 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700/50">
                        <div className="flex items-center gap-2">
                          <div
                            className="w-8 h-8 rounded-lg flex items-center justify-center"
                            style={{ backgroundColor: `${config.color}20` }}
                          >
                            <Icon className="w-4 h-4" style={{ color: config.color }} />
                          </div>
                          <span className="text-sm font-medium text-gray-700 dark:text-gray-300">
                            {config.label}
                          </span>
                        </div>
                        <span className="text-sm font-semibold text-gray-900 dark:text-white">
                          {formatCurrency(amount)}
                        </span>
                      </div>
                    );
                  })}
                </div>
              </CardContent>
            </Card>
          </div>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>Recent Expenses</CardTitle>
          </CardHeader>
          <CardContent>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Date</TableHead>
                  <TableHead>Category</TableHead>
                  <TableHead>Vendor</TableHead>
                  <TableHead>Amount</TableHead>
                  <TableHead>Notes</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {mockExpenses.map((expense) => {
                  const Icon = categoryConfig[expense.category as keyof typeof categoryConfig].icon;
                  const color = categoryConfig[expense.category as keyof typeof categoryConfig].color;
                  return (
                    <TableRow key={expense.id}>
                      <TableCell>
                        <span className="text-sm font-medium">{formatDate(expense.expenseDate)}</span>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <div
                            className="w-8 h-8 rounded-lg flex items-center justify-center"
                            style={{ backgroundColor: `${color}20` }}
                          >
                            <Icon className="w-4 h-4" style={{ color }} />
                          </div>
                          <span className="font-medium">
                            {categoryConfig[expense.category as keyof typeof categoryConfig].label}
                          </span>
                        </div>
                      </TableCell>
                      <TableCell>{expense.vendor}</TableCell>
                      <TableCell>
                        <span className="font-semibold text-red-600 dark:text-red-400">
                          {formatCurrency(expense.amount)}
                        </span>
                      </TableCell>
                      <TableCell>
                        <span className="text-sm text-gray-600 dark:text-gray-400">
                          {expense.notes}
                        </span>
                      </TableCell>
                      <TableCell>
                        <Button variant="ghost" size="sm">
                          Edit
                        </Button>
                      </TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
          </CardContent>
        </Card>
      </div>

      <Modal
        isOpen={showAddModal}
        onClose={() => {
          setShowAddModal(false);
          reset();
        }}
        title="Add Expense"
        size="lg"
      >
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <Select
            label="Category"
            options={[
              { value: '', label: 'Select category...' },
              ...Object.entries(categoryConfig).map(([key, config]) => ({
                value: key,
                label: config.label,
              })),
            ]}
            error={errors.category?.message}
            {...register('category')}
          />

          <Input
            label="Vendor Name"
            placeholder="e.g., City Power Co."
            error={errors.vendor?.message}
            {...register('vendor')}
          />

          <div className="grid grid-cols-2 gap-4">
            <Input
              label="Amount (₹)"
              type="number"
              placeholder="5000"
              error={errors.amount?.message}
              {...register('amount')}
            />

            <Input
              label="Expense Date"
              type="date"
              error={errors.expenseDate?.message}
              {...register('expenseDate')}
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Notes
            </label>
            <textarea
              rows={3}
              placeholder="Additional details about this expense..."
              className="w-full px-4 py-2.5 rounded-xl border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-primary-500"
              {...register('notes')}
            />
            {errors.notes && (
              <p className="mt-1.5 text-sm text-red-600 dark:text-red-400">{errors.notes.message}</p>
            )}
          </div>

          <ModalFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => {
                setShowAddModal(false);
                reset();
              }}
            >
              Cancel
            </Button>
            <Button type="submit">
              <Plus className="w-4 h-4 mr-2" />
              Add Expense
            </Button>
          </ModalFooter>
        </form>
      </Modal>
    </DashboardLayout>
  );
}
