'use client'

import { useState } from 'react'
import { Copy, Check } from 'lucide-react'
import { cn } from '@/lib/utils'

interface CopyButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  text: string
  variant?: 'ghost' | 'outline' | 'default'
  size?: 'sm' | 'md'
  showText?: boolean
}

export function CopyButton({
  text,
  variant = 'ghost',
  size = 'sm',
  showText = false,
  className,
  ...props
}: CopyButtonProps) {
  const [copied, setCopied] = useState(false)

  const handleCopy = async () => {
    await navigator.clipboard.writeText(text)
    setCopied(true)
    setTimeout(() => setCopied(false), 2000)
  }

  const sizeClasses = {
    sm: 'h-8 w-8 p-1.5',
    md: 'h-10 w-10 p-2',
  }

  const variantClasses = {
    ghost: 'hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-600 dark:text-slate-400',
    outline: 'border border-slate-200 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-900 text-slate-600 dark:text-slate-400',
    default: 'bg-slate-100 dark:bg-slate-800 text-slate-700 dark:text-slate-300 hover:bg-slate-200 dark:hover:bg-slate-700',
  }

  return (
    <button
      onClick={handleCopy}
      className={cn(
        'inline-flex items-center gap-1.5 rounded-lg transition-smooth duration-150 font-medium text-xs',
        sizeClasses[size],
        variantClasses[variant],
        'hover:shadow-sm active:scale-95',
        className
      )}
      title={copied ? 'Copied!' : 'Copy to clipboard'}
      {...props}
    >
      {copied ? (
        <>
          <Check className="w-4 h-4" />
          {showText && <span>Copied</span>}
        </>
      ) : (
        <>
          <Copy className="w-4 h-4" />
          {showText && <span>Copy</span>}
        </>
      )}
    </button>
  )
}

interface CopyFieldProps {
  label?: string
  value: string
  className?: string
}

export function CopyField({ label, value, className }: CopyFieldProps) {
  return (
    <div className={cn('flex items-center gap-2', className)}>
      <div className="flex-1">
        {label && (
          <p className="text-xs font-medium text-slate-600 dark:text-slate-400 mb-1">
            {label}
          </p>
        )}
        <p className="font-mono text-sm text-slate-700 dark:text-slate-300 break-all">
          {value}
        </p>
      </div>
      <CopyButton text={value} variant="outline" size="md" className="flex-shrink-0" />
    </div>
  )
}

export function CopyInline({ text, className }: { text: string; className?: string }) {
  return (
    <span className={cn('inline-flex items-center gap-1', className)}>
      <code className="font-mono text-xs bg-slate-100 dark:bg-slate-800 px-2 py-1 rounded">
        {text}
      </code>
      <CopyButton text={text} variant="ghost" size="sm" />
    </span>
  )
}
