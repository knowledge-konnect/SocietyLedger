import { useState } from 'react';
import { Plus, CreditCard, Download, Search, Upload } from 'lucide-react';
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

const paymentSchema = z.object({
  flatId: z.string().min(1, 'Please select a flat'),
  amount: z.string().min(1, 'Amount is required'),
  paymentMode: z.string(),
  paymentDate: z.string(),
  referenceNumber: z.string().optional(),
  notes: z.string().optional(),
});

type PaymentFormData = z.infer<typeof paymentSchema>;

const mockPayments = [
  { id: '1', flatNumber: 'A-101', ownerName: 'Rajesh Kumar', amount: 5000, paymentDate: '2025-01-08', paymentMode: 'cash' as const, referenceNumber: '', billNumber: 'INV-2025-001' },
  { id: '2', flatNumber: 'B-201', ownerName: 'Amit Patel', amount: 2000, paymentDate: '2025-01-12', paymentMode: 'cheque' as const, referenceNumber: 'CHQ789012', billNumber: 'INV-2025-003' },
  { id: '3', flatNumber: 'C-301', ownerName: 'Vikram Singh', amount: 4500, paymentDate: '2025-01-15', paymentMode: 'cheque' as const, referenceNumber: 'CHQ345678', billNumber: 'INV-2025-005' },
  { id: '4', flatNumber: 'A-102', ownerName: 'Priya Sharma', amount: 5000, paymentDate: '2025-01-18', paymentMode: 'cash' as const, referenceNumber: '', billNumber: 'INV-2025-002' },
];

const paymentModeLabels = {
  cash: 'Cash',
  cheque: 'Cheque',
};

export default function Payments() {
  const [showAddModal, setShowAddModal] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');

  const { register, handleSubmit, formState: { errors }, reset } = useForm<PaymentFormData>({
    resolver: zodResolver(paymentSchema),
  });

  const onSubmit = (data: PaymentFormData) => {
    console.log('Payment data:', data);
    setShowAddModal(false);
    reset();
  };

  const filteredPayments = searchQuery
    ? mockPayments.filter(p =>
        p.flatNumber.toLowerCase().includes(searchQuery.toLowerCase()) ||
        p.ownerName.toLowerCase().includes(searchQuery.toLowerCase())
      )
    : mockPayments;

  const totalCollected = mockPayments.reduce((sum, p) => sum + p.amount, 0);

  return (
    <DashboardLayout title="Payments">
      <div className="space-y-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <Card className="bg-gradient-to-br from-green-50 to-emerald-100 dark:from-green-900/20 dark:to-emerald-800/20 border-green-200 dark:border-green-800">
            <CardContent className="p-6">
              <div className="flex items-center justify-between mb-4">
                <div className="w-14 h-14 bg-green-500 rounded-2xl flex items-center justify-center shadow-lg">
                  <CreditCard className="w-7 h-7 text-white" />
                </div>
              </div>
              <p className="text-sm text-green-700 dark:text-green-300 font-medium mb-2">Total Collected (MTD)</p>
              <p className="text-3xl font-bold text-green-900 dark:text-green-100">
                {formatCurrency(totalCollected)}
              </p>
              <p className="text-xs text-green-600 dark:text-green-400 mt-2">
                {mockPayments.length} transactions
              </p>
            </CardContent>
          </Card>

          <Card className="bg-gradient-to-br from-blue-50 to-cyan-100 dark:from-blue-900/20 dark:to-cyan-800/20 border-blue-200 dark:border-blue-800">
            <CardContent className="p-6">
              <div className="w-14 h-14 bg-blue-500 rounded-2xl flex items-center justify-center shadow-lg mb-4">
                <CreditCard className="w-7 h-7 text-white" />
              </div>
              <p className="text-sm text-blue-700 dark:text-blue-300 font-medium mb-2">Offline Payments</p>
              <p className="text-3xl font-bold text-blue-900 dark:text-blue-100">
                {mockPayments.filter(p => ['cash', 'cheque'].includes(p.paymentMode)).length}
              </p>
              <p className="text-xs text-blue-600 dark:text-blue-400 mt-2">
                {Math.round((mockPayments.filter(p => ['cash', 'cheque'].includes(p.paymentMode)).length / mockPayments.length) * 100)}% of total
              </p>
            </CardContent>
          </Card>

          <Card className="bg-gradient-to-br from-purple-50 to-pink-100 dark:from-purple-900/20 dark:to-pink-800/20 border-purple-200 dark:border-purple-800">
            <CardContent className="p-6">
              <div className="w-14 h-14 bg-purple-500 rounded-2xl flex items-center justify-center shadow-lg mb-4">
                <CreditCard className="w-7 h-7 text-white" />
              </div>
              <p className="text-sm text-purple-700 dark:text-purple-300 font-medium mb-2">Cash/Cheque</p>
              <p className="text-3xl font-bold text-purple-900 dark:text-purple-100">
                {mockPayments.filter(p => ['cash', 'cheque'].includes(p.paymentMode)).length}
              </p>
              <p className="text-xs text-purple-600 dark:text-purple-400 mt-2">
                {Math.round((mockPayments.filter(p => ['cash', 'cheque'].includes(p.paymentMode)).length / mockPayments.length) * 100)}% of total
              </p>
            </CardContent>
          </Card>
        </div>

        <Card>
          <CardHeader>
            <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
              <CardTitle>Payment History</CardTitle>
              <div className="flex gap-3 w-full sm:w-auto">
                <div className="relative flex-1 sm:flex-initial">
                  <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
                  <input
                    type="text"
                    placeholder="Search payments..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    className="w-full pl-10 pr-4 py-2 rounded-xl border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500"
                  />
                </div>
                <Button size="sm" onClick={() => setShowAddModal(true)}>
                  <Plus className="w-4 h-4 mr-2" />
                  Record Payment
                </Button>
              </div>
            </div>
          </CardHeader>
          <CardContent>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Date</TableHead>
                  <TableHead>Flat Number</TableHead>
                  <TableHead>Owner Name</TableHead>
                  <TableHead>Bill Number</TableHead>
                  <TableHead>Amount</TableHead>
                  <TableHead>Payment Mode</TableHead>
                  <TableHead>Reference</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredPayments.map((payment) => (
                  <TableRow key={payment.id}>
                    <TableCell>
                      <span className="text-sm font-semibold text-foreground">{formatDate(payment.paymentDate)}</span>
                    </TableCell>
                    <TableCell>
                      <span className="font-bold text-primary">
                        {payment.flatNumber}
                      </span>
                    </TableCell>
                    <TableCell className="font-semibold text-foreground">{payment.ownerName}</TableCell>
                    <TableCell>
                      <span className="text-sm text-muted-foreground">
                        {payment.billNumber}
                      </span>
                    </TableCell>
                    <TableCell>
                      <span className="font-bold text-success">
                        {formatCurrency(payment.amount)}
                      </span>
                    </TableCell>
                    <TableCell>
                      <Badge variant="default">
                        {paymentModeLabels[payment.paymentMode as keyof typeof paymentModeLabels]}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <span className="text-sm font-mono text-muted-foreground">
                        {payment.referenceNumber || '-'}
                      </span>
                    </TableCell>
                    <TableCell>
                      <Button variant="ghost" size="sm">
                        <Download className="w-4 h-4 mr-1" />
                        Receipt
                      </Button>
                    </TableCell>
                  </TableRow>
                ))}
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
        title="Record Payment"
        size="lg"
      >
        <form onSubmit={handleSubmit(onSubmit)} className="form-group space-y-4 md:space-y-6">
          <div className="form-field">
            <Select
              label="Select Flat"
              options={[
                { value: '', label: 'Choose a flat...' },
                { value: '1', label: 'A-101 - Rajesh Kumar' },
                { value: '2', label: 'A-102 - Priya Sharma' },
                { value: '3', label: 'B-201 - Amit Patel' },
              ]}
              error={errors.flatId?.message}
              {...register('flatId')}
            />
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 md:gap-6">
            <div className="form-field">
              <Input
                label="Amount (₹)"
                type="number"
                placeholder="5000"
                error={errors.amount?.message}
                {...register('amount')}
              />
            </div>

            <div className="form-field">
              <Input
                label="Payment Date"
                type="date"
                error={errors.paymentDate?.message}
                {...register('paymentDate')}
              />
            </div>
          </div>

          <div className="form-field">
            <Select
              label="Payment Mode"
              options={[
                { value: '', label: 'Choose a payment mode...' },
                { value: 'cash', label: 'Cash' },
                { value: 'cheque', label: 'Cheque' },
              ]}
              error={errors.paymentMode?.message}
              {...register('paymentMode')}
            />
          </div>

          <div className="form-field">
            <Input
              label="Reference Number (Optional)"
              placeholder="Transaction ID, Cheque number, etc."
              error={errors.referenceNumber?.message}
              {...register('referenceNumber')}
            />
          </div>

          <div className="form-field">
            <label className="block text-sm font-semibold text-foreground mb-2">
              Upload Receipt (Optional)
            </label>
            <div className="border-2 border-dashed border-border rounded-xl p-6 text-center hover:border-primary/50 transition-colors cursor-pointer bg-muted/20">
              <Upload className="w-8 h-8 text-muted-foreground mx-auto mb-2" />
              <p className="text-sm text-muted-foreground">
                Click to upload or drag and drop
              </p>
              <p className="text-xs text-muted-foreground/60 mt-1">
                PNG, JPG or PDF (max. 5MB)
              </p>
            </div>
          </div>

          <div className="form-field">
            <Input
              label="Notes (Optional)"
              placeholder="Additional notes..."
              error={errors.notes?.message}
              {...register('notes')}
            />
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
              <CreditCard className="w-4 h-4 mr-2" />
              Record Payment
            </Button>
          </ModalFooter>
        </form>
      </Modal>
    </DashboardLayout>
  );
}
