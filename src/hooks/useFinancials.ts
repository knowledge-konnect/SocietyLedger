import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { financialsApi, CreateAccountDto, CreateTransactionDto } from '../api/financialsApi';
import { PaginationParams } from '../types/api';

// Accounts
export const useAccounts = (params?: PaginationParams) => {
  return useQuery({
    queryKey: ['accounts', params],
    queryFn: () => financialsApi.getAccounts(params)
  });
};

export const useCreateAccount = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateAccountDto) => financialsApi.createAccount(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['accounts'] });
    }
  });
};

// Transactions
export const useTransactions = (params?: PaginationParams) => {
  return useQuery({
    queryKey: ['transactions', params],
    queryFn: () => financialsApi.getTransactions(params)
  });
};

export const useCreateTransaction = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateTransactionDto) => financialsApi.createTransaction(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['transactions'] });
      queryClient.invalidateQueries({ queryKey: ['accounts'] }); // Invalidate accounts as balances may change
    }
  });
};

// Invoices
export const useInvoices = (params?: PaginationParams) => {
  return useQuery({
    queryKey: ['invoices', params],
    queryFn: () => financialsApi.getInvoices(params)
  });
};

export const useInvoice = (id: string) => {
  return useQuery({
    queryKey: ['invoices', id],
    queryFn: () => financialsApi.getInvoiceById(id),
    enabled: !!id
  });
};

export const useRecordPayment = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (invoiceId: string) => financialsApi.recordPayment(invoiceId),
    onSuccess: (_, invoiceId) => {
      queryClient.invalidateQueries({ queryKey: ['invoices'] });
      queryClient.invalidateQueries({ queryKey: ['invoices', invoiceId] });
      queryClient.invalidateQueries({ queryKey: ['transactions'] });
      queryClient.invalidateQueries({ queryKey: ['accounts'] });
    }
  });
};

// Financial Reports
export const useFinancialReport = (startDate: string, endDate: string) => {
  return useQuery({
    queryKey: ['financial-report', startDate, endDate],
    queryFn: () => financialsApi.getFinancialReport(startDate, endDate),
    enabled: !!startDate && !!endDate
  });
};
