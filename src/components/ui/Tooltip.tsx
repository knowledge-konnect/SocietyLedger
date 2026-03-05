import { ReactNode } from 'react';

export interface TooltipProps {
  content: ReactNode;
  side?: 'top' | 'right' | 'bottom' | 'left';
  children: ReactNode;
}

export default function Tooltip({ content, side = 'top', children }: TooltipProps) {
  const posClass = {
    top: 'bottom-full left-1/2 -translate-x-1/2 mb-3',
    bottom: 'top-full left-1/2 -translate-x-1/2 mt-3',
    left: 'right-full top-1/2 -translate-y-1/2 mr-3',
    right: 'left-full top-1/2 -translate-y-1/2 ml-3',
  }[side];

  return (
    <div className="group inline-flex relative">
      {children}
      <div
        role="tooltip"
        className={`pointer-events-none absolute z-50 ${posClass} opacity-0 scale-95 transform transition-all duration-200 bg-slate-900 dark:bg-slate-950 text-white text-xs font-bold rounded-lg px-3 py-2 whitespace-nowrap shadow-lg group-hover:opacity-100 group-focus:opacity-100 group-hover:scale-100`}
      >
        {content}
        <div className={`absolute w-2 h-2 bg-slate-900 dark:bg-slate-950 transform rotate-45 ${
          side === 'top' ? '-bottom-1 left-1/2 -translate-x-1/2' :
          side === 'bottom' ? '-top-1 left-1/2 -translate-x-1/2' :
          side === 'left' ? '-right-1 top-1/2 -translate-y-1/2' :
          '-left-1 top-1/2 -translate-y-1/2'
        }`} />
      </div>
    </div>
  );
}
