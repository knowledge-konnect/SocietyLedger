import React from 'react';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import { cn } from '../../lib/utils';

interface Props {
  page: number;
  pageSize: number;
  total: number;
  onPageChange: (newPage: number) => void;
  onPageSizeChange?: (size: number) => void;
}

export default function Pagination({ page, pageSize, total, onPageChange, onPageSizeChange }: Props) {
  const totalPages = Math.max(1, Math.ceil(total / pageSize));

  return (
    <div className="flex flex-col sm:flex-row items-center justify-between gap-4 mt-6 p-4 bg-slate-100/50 dark:bg-slate-800/30 rounded-lg">
      <div className="text-sm text-slate-600 dark:text-slate-400 font-medium">
        Showing <strong className="text-slate-900 dark:text-white">{Math.min(total, page * pageSize + 1)}</strong> - <strong className="text-slate-900 dark:text-white">{Math.min(total, (page + 1) * pageSize)}</strong> of <strong className="text-slate-900 dark:text-white">{total}</strong>
      </div>

      <div className="flex items-center gap-3 flex-wrap justify-center sm:justify-end">
        <select
          value={pageSize}
          onChange={(e) => onPageSizeChange && onPageSizeChange(Number(e.target.value))}
          className={cn(
            'px-3 py-2 rounded-lg border-2 border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-900 text-slate-900 dark:text-white text-sm font-medium',
            'focus:outline-none focus:ring-2 focus:ring-indigo-500/50 focus:border-indigo-500 dark:focus:border-indigo-500',
            'transition-all duration-200 hover:border-slate-300 dark:hover:border-slate-600',
            'disabled:opacity-50 disabled:cursor-not-allowed'
          )}
          aria-label="Items per page"
        >
          {[10, 25, 50, 100].map(s => (
            <option key={s} value={s}>{s} per page</option>
          ))}
        </select>

        <div className="flex items-center gap-2">
          <button
            onClick={() => onPageChange(Math.max(0, page - 1))}
            disabled={page === 0}
            className={cn(
              'inline-flex items-center justify-center gap-1.5 px-3 py-2 rounded-lg text-sm font-medium',
              'border-2 border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-900 text-slate-900 dark:text-white transition-all duration-200',
              'hover:border-indigo-300 dark:hover:border-indigo-600 hover:bg-indigo-50 dark:hover:bg-indigo-900/20',
              'focus:outline-none focus:ring-2 focus:ring-indigo-500/50',
              'disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:border-slate-200 dark:disabled:hover:border-slate-700 disabled:hover:bg-white dark:disabled:hover:bg-slate-900',
              'active:scale-95'
            )}
            aria-label="Previous page"
          >
            <ChevronLeft className="w-4 h-4" />
            <span className="hidden sm:inline">Prev</span>
          </button>

          <div className="px-3 py-2 rounded-lg bg-slate-100 dark:bg-slate-800 text-center text-sm font-medium text-slate-900 dark:text-white min-w-12">
            <span>{page + 1}</span>
            <span className="text-slate-600 dark:text-slate-400 mx-1">/</span>
            <span>{totalPages}</span>
          </div>

          <button
            onClick={() => onPageChange(Math.min(totalPages - 1, page + 1))}
            disabled={page >= totalPages - 1}
            className={cn(
              'inline-flex items-center justify-center gap-1.5 px-3 py-2 rounded-lg text-sm font-medium',
              'border-2 border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-900 text-slate-900 dark:text-white transition-all duration-200',
              'hover:border-indigo-300 dark:hover:border-indigo-600 hover:bg-indigo-50 dark:hover:bg-indigo-900/20',
              'focus:outline-none focus:ring-2 focus:ring-indigo-500/50',
              'disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:border-slate-200 dark:disabled:hover:border-slate-700 disabled:hover:bg-white dark:disabled:hover:bg-slate-900',
              'active:scale-95'
            )}
            aria-label="Next page"
          >
            <span className="hidden sm:inline">Next</span>
            <ChevronRight className="w-4 h-4" />
          </button>
        </div>
      </div>
    </div>
  );
}
