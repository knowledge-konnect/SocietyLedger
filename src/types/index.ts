export interface Society {
  id: string;
  name: string;
  address: string;
  billingCycle: 'monthly' | 'quarterly' | 'yearly';
  totalFlats: number;
  createdAt: string;
}

export interface Flat {
  id: string;
  societyId: string;
  flatNumber: string;
  ownerName: string;
  ownerEmail: string;
  ownerPhone: string;
  maintenanceAmount: number;
  outstandingBalance: number;
  status: 'active' | 'inactive';
}

export interface Bill {
  id: string;
  flatId: string;
  societyId: string;
  billNumber: string;
  billDate: string;
  dueDate: string;
  amount: number;
  status: 'pending' | 'paid' | 'overdue' | 'partial';
  paidAmount: number;
  paidDate?: string;
}

export interface Payment {
  id: string;
  billId: string;
  flatId: string;
  societyId: string;
  amount: number;
  paymentDate: string;
  paymentMode: 'cash' | 'cheque';
  referenceNumber?: string;
  notes?: string;
  receiptUrl?: string;
}

export interface Expense {
  id: string;
  societyId: string;
  category: 'electricity' | 'water' | 'security' | 'repairs' | 'salary' | 'others';
  vendor: string;
  amount: number;
  expenseDate: string;
  notes?: string;
  receiptUrl?: string;
}

export interface User {
  id: string;
  email: string;
  name: string;
  role: 'admin' | 'treasurer' | 'member';
  societyId: string;
  status: 'active' | 'inactive';
}

export interface DashboardStats {
  totalFlats: number;
  totalDue: number;
  totalCollected: number;
  totalExpenses: number;
  currentBalance: number;
  collectionRate: number;
}

export interface ChartData {
  month: string;
  income: number;
  expense: number;
}

export interface Activity {
  id: string;
  type: 'payment' | 'bill' | 'expense';
  description: string;
  amount: number;
  timestamp: string;
}
