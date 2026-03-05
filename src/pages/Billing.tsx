import { useState } from 'react';
import { FileText, Calendar, Download, Send, AlertCircle, CheckCircle2, Clock } from 'lucide-react';
import DashboardLayout from '../components/layout/DashboardLayout';
import Button from '../components/ui/Button';
import Modal from '../components/ui/Modal';
import { Table, TableHeader, TableBody, TableRow, TableHead, TableCell } from '../components/ui/Table';
import Badge from '../components/ui/Badge';
import Select from '../components/ui/Select';
import { formatCurrency, formatDate } from '../lib/utils';
import { useInvoices } from '../hooks/useFinancials';
import { Invoice } from '../api/financialsApi';
import { useToast } from '../components/ui/Toast';

const mockBills = [
  { id: '1', billNumber: 'INV-2025-001', flatNumber: 'A-101', ownerName: 'Rajesh Kumar', amount: 5000, paidAmount: 5000, status: 'paid', billDate: '2025-01-01', dueDate: '2025-01-10', paidDate: '2025-01-08' },
  { id: '2', billNumber: 'INV-2025-002', flatNumber: 'A-102', ownerName: 'Priya Sharma', amount: 5000, paidAmount: 0, status: 'overdue', billDate: '2025-01-01', dueDate: '2025-01-10', paidDate: null },
  { id: '3', billNumber: 'INV-2025-003', flatNumber: 'B-201', ownerName: 'Amit Patel', amount: 4500, paidAmount: 2000, status: 'partial', billDate: '2025-01-01', dueDate: '2025-01-10', paidDate: null },
  { id: '4', billNumber: 'INV-2025-004', flatNumber: 'B-202', ownerName: 'Sneha Gupta', amount: 4500, paidAmount: 0, status: 'pending', billDate: '2025-01-01', dueDate: '2025-01-25', paidDate: null },
];

const months = [
  { value: '2025-01', label: 'January 2025' },
  { value: '2024-12', label: 'December 2024' },
  { value: '2024-11', label: 'November 2024' },
];

const statusColors = {
  paid: 'success',
  pending: 'warning',
  overdue: 'destructive',
  partial: 'info',
} as const;

export default function Billing() {
  const [showGenerateModal, setShowGenerateModal] = useState(false);
  const [selectedPeriod, setSelectedPeriod] = useState('2025-01');
  const [filterStatus, setFilterStatus] = useState('all');

  const filteredBills = filterStatus === 'all'
    ? mockBills
    : mockBills.filter(b => b.status === filterStatus);

  const stats = {
    totalBills: mockBills.length,
    totalAmount: mockBills.reduce((sum, b) => sum + b.amount, 0),
    totalPaid: mockBills.reduce((sum, b) => sum + b.paidAmount, 0),
    totalPending: mockBills.reduce((sum, b) => sum + (b.amount - b.paidAmount), 0),
  };

  return (
    <DashboardLayout title="Billing & Invoices">
      <div className="space-y-8">
        <div>
          <h2 className="section-heading mb-2">Billing & Invoices</h2>
          <p className="subheading">Manage bills, track payments, and view collection details</p>
        </div>

        <div className="grid-auto-fit-lg">
          <div className="card-base p-6 group card-hover bg-gradient-to-br from-info/10 to-info/5 animate-slide-in-up" style={{ animationDelay: '0s' }}>
            <div className="flex items-start justify-between mb-6">
              <div className="w-12 h-12 gradient-primary rounded-lg flex items-center justify-center group-hover:scale-110 transition-transform duration-300">
                <FileText className="w-6 h-6 text-white" />
              </div>
              <span className="badge-primary text-xs">{stats.totalBills} bills</span>
            </div>
            <p className="text-sm text-muted-foreground mb-2">Total Billed</p>
            <p className="text-3xl font-bold text-foreground">{formatCurrency(stats.totalAmount)}</p>
          </div>

          <div className="card-base p-6 group card-hover bg-gradient-to-br from-success/10 to-success/5 animate-slide-in-up" style={{ animationDelay: '0.05s' }}>
            <div className="flex items-start justify-between mb-6">
              <div className="w-12 h-12 bg-success rounded-lg flex items-center justify-center group-hover:scale-110 transition-transform duration-300">
                <CheckCircle2 className="w-6 h-6 text-white" />
              </div>
              <span className="badge-success text-xs">Collected</span>
            </div>
            <p className="text-sm text-muted-foreground mb-2">Total Paid</p>
            <p className="text-3xl font-bold text-foreground">{formatCurrency(stats.totalPaid)}</p>
          </div>

          <div className="card-base p-6 group card-hover bg-gradient-to-br from-warning/10 to-warning/5 animate-slide-in-up" style={{ animationDelay: '0.1s' }}>
            <div className="flex items-start justify-between mb-6">
              <div className="w-12 h-12 bg-warning rounded-lg flex items-center justify-center group-hover:scale-110 transition-transform duration-300">
                <Clock className="w-6 h-6 text-white" />
              </div>
              <span className="badge-warning text-xs">Pending</span>
            </div>
            <p className="text-sm text-muted-foreground mb-2">Outstanding</p>
            <p className="text-3xl font-bold text-foreground">{formatCurrency(stats.totalPending)}</p>
          </div>

          <div className="card-base p-6 group card-hover bg-gradient-to-br from-primary/10 to-primary/5">
            <div className="flex items-start justify-between mb-6">
              <div className="w-12 h-12 gradient-primary rounded-lg flex items-center justify-center group-hover:scale-110 transition-transform duration-300">
                <AlertCircle className="w-6 h-6 text-white" />
              </div>
              <span className="badge-primary text-xs">Rate</span>
            </div>
            <p className="text-sm text-muted-foreground mb-2">Collection Rate</p>
            <p className="text-3xl font-bold text-foreground">{Math.round((stats.totalPaid / stats.totalAmount) * 100)}%</p>
          </div>
        </div>

        <div className="card-base p-8">
          <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 mb-8">
            <div>
              <h3 className="text-xl font-bold text-foreground">Bills & Invoices</h3>
              <p className="text-sm text-muted-foreground mt-1">Track all generated bills and payment status</p>
            </div>
            <div className="flex gap-3 w-full sm:w-auto">
              <Select
                options={[
                  { value: 'all', label: 'All Status' },
                  { value: 'paid', label: 'Paid' },
                  { value: 'pending', label: 'Pending' },
                  { value: 'overdue', label: 'Overdue' },
                  { value: 'partial', label: 'Partial' },
                ]}
                value={filterStatus}
                onChange={(e) => setFilterStatus(e.target.value)}
              />
              <Button variant="primary" size="sm" onClick={() => setShowGenerateModal(true)} className="flex-shrink-0">
                <Calendar className="w-4 h-4 mr-2" />
                Generate
              </Button>
            </div>
          </div>

          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Bill</TableHead>
                  <TableHead className="hidden md:table-cell">Flat</TableHead>
                  <TableHead>Owner</TableHead>
                  <TableHead className="text-right">Amount</TableHead>
                  <TableHead className="hidden lg:table-cell text-right">Paid</TableHead>
                  <TableHead className="hidden xl:table-cell">Due</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead className="text-right">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredBills.map((bill) => (
                  <TableRow key={bill.id}>
                    <TableCell>
                      <span className="font-semibold text-indigo-600 dark:text-indigo-400">{bill.billNumber}</span>
                    </TableCell>
                    <TableCell className="hidden md:table-cell">
                      <span className="font-medium">{bill.flatNumber}</span>
                    </TableCell>
                    <TableCell className="text-sm">{bill.ownerName}</TableCell>
                    <TableCell className="text-right font-semibold">{formatCurrency(bill.amount)}</TableCell>
                    <TableCell className="hidden lg:table-cell text-right">
                      <span className={bill.paidAmount > 0 ? 'text-green-600 dark:text-green-400 font-semibold' : 'text-slate-500'}>
                        {formatCurrency(bill.paidAmount)}
                      </span>
                    </TableCell>
                    <TableCell className="hidden xl:table-cell text-sm">
                      <span className={new Date(bill.dueDate) < new Date() && bill.status !== 'paid' ? 'text-red-600 dark:text-red-400 font-semibold' : ''}>
                        {formatDate(bill.dueDate)}
                      </span>
                    </TableCell>
                    <TableCell>
                      <Badge variant={statusColors[bill.status]}>
                        {bill.status}
                      </Badge>
                    </TableCell>
                    <TableCell className="text-right">
                      <div className="flex gap-1 justify-end">
                        <Button variant="ghost" size="sm">
                          <Download className="w-4 h-4" />
                        </Button>
                        {bill.status !== 'paid' && (
                          <Button variant="ghost" size="sm">
                            <Send className="w-4 h-4" />
                          </Button>
                        )}
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </div>
      </div>

      <Modal
        isOpen={showGenerateModal}
        onClose={() => setShowGenerateModal(false)}
        title="Generate Monthly Bills"
        size="lg"
      >
        <div className="space-y-6">
          <div className="card-premium p-6">
            <h4 className="font-semibold text-foreground mb-4">Billing Summary</h4>
            <div className="grid grid-cols-2 gap-6">
              <div>
                <p className="text-sm text-muted-foreground">Total Flats</p>
                <p className="text-3xl font-bold text-foreground mt-2">120</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total Amount</p>
                <p className="text-3xl font-bold text-primary mt-2">{formatCurrency(545000)}</p>
              </div>
            </div>
          </div>

          <Select
            label="Select Billing Period"
            options={months}
            value={selectedPeriod}
            onChange={(e) => setSelectedPeriod(e.target.value)}
          />

          <div className="p-4 bg-warning/10 border border-warning/30 rounded-lg">
            <div className="flex gap-3">
              <AlertCircle className="w-5 h-5 text-warning flex-shrink-0 mt-0.5" />
              <div>
                <p className="text-sm font-medium text-warning">Important Notice</p>
                <p className="text-sm text-muted-foreground mt-1">
                  Bills will be generated for all active flats. This action cannot be undone. Existing bills for this period will not be affected.
                </p>
              </div>
            </div>
          </div>

          <div className="flex gap-3 justify-end pt-4">
            <Button variant="outline" onClick={() => setShowGenerateModal(false)}>
              Cancel
            </Button>
            <Button variant="primary">
              <FileText className="w-4 h-4 mr-2" />
              Generate 120 Bills
            </Button>
          </div>
        </div>
      </Modal>
    </DashboardLayout>
  );
}
