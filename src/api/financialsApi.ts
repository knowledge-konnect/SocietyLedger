import { PaginatedResponse, PaginationParams, ApiResponse } from '../types/api';
import apiClient from './client';

export interface Account {
  id: string;
  code: string;
  name: string;
  type: 'ASSET' | 'LIABILITY' | 'INCOME' | 'EXPENSE';
  balance: number;
}

export interface Transaction {
  id: string;
  date: string;
  description: string;
  amount: number;
  type: 'CREDIT' | 'DEBIT';
  accountId: string;
  createdAt: string;
}

export interface Invoice {
  id: string;
  number: string;
  date: string;
  dueDate: string;
  amount: number;
  status: 'PENDING' | 'PAID' | 'OVERDUE';
  items: InvoiceItem[];
}

export interface InvoiceItem {
  description: string;
  quantity: number;
  unitPrice: number;
  amount: number;
}

export interface CreateAccountDto {
  code: string;
  name: string;
  type: Account['type'];
}

export interface CreateTransactionDto {
  date: string;
  description: string;
  amount: number;
  type: Transaction['type'];
  accountId: string;
}

export interface FinancialReport {
  startDate: string;
  endDate: string;
  income: number;
  expenses: number;
  profit: number;
  cashflow: {
    opening: number;
    closing: number;
    net: number;
  };
}

export const financialsApi = {
  // Accounts
  async getAccounts(params?: PaginationParams): Promise<PaginatedResponse<Account>> {
    const response = await apiClient.get<ApiResponse<PaginatedResponse<Account>>>('/accounts', { params });
    return response.data.data;
  },

  async createAccount(payload: CreateAccountDto): Promise<Account> {
    const response = await apiClient.post<ApiResponse<Account>>('/accounts', payload);
    return response.data.data;
  },

  // Transactions
  async getTransactions(params?: PaginationParams): Promise<PaginatedResponse<Transaction>> {
    const response = await apiClient.get<ApiResponse<PaginatedResponse<Transaction>>>('/transactions', { params });
    return response.data.data;
  },

  async createTransaction(payload: CreateTransactionDto): Promise<Transaction> {
    const response = await apiClient.post<ApiResponse<Transaction>>('/transactions', payload);
    return response.data.data;
  },

  // Invoices
  async getInvoices(params?: PaginationParams): Promise<PaginatedResponse<Invoice>> {
    const response = await apiClient.get<ApiResponse<PaginatedResponse<Invoice>>>('/invoices', { params });
    return response.data.data;
  },

  async getInvoiceById(id: string): Promise<Invoice> {
    const response = await apiClient.get<ApiResponse<Invoice>>(`/invoices/${id}`);
    return response.data.data;
  },

  async recordPayment(id: string): Promise<Invoice> {
    const response = await apiClient.post<ApiResponse<Invoice>>(`/invoices/${id}/pay`);
    return response.data.data;
  },

  // Reports
  async getFinancialReport(startDate: string, endDate: string): Promise<FinancialReport> {
    const response = await apiClient.get<ApiResponse<FinancialReport>>('/reports/financial', {
      params: { startDate, endDate }
    });
    return response.data.data;
  }
};
