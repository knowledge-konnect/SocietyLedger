'use client'

import { useState } from 'react'
import { AlertTriangle, Trash2 } from 'lucide-react'
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from '@/components/ui/alert-dialog'
import { Button, type buttonVariants } from '@/components/ui/button'
import { type VariantProps } from 'class-variance-authority'

interface ConfirmDialogProps {
  title: string
  description: string
  actionLabel?: string
  cancelLabel?: string
  onConfirm: () => Promise<void> | void
  onCancel?: () => void
  isDangerous?: boolean
  children?: React.ReactNode
  trigger?: React.ReactNode
  triggerVariant?: VariantProps<typeof buttonVariants>['variant']
  triggerSize?: VariantProps<typeof buttonVariants>['size']
  triggerText?: string
  open?: boolean
  onOpenChange?: (open: boolean) => void
}

export function ConfirmDialog(props: ConfirmDialogProps) {
  const {
    title,
    description,
    actionLabel = 'Confirm',
    cancelLabel = 'Cancel',
    onConfirm,
    onCancel,
    isDangerous = false,
    children,
    trigger,
    triggerVariant = 'outline',
    triggerSize = 'default',
    triggerText = 'Delete',
    open: controlledOpen,
    onOpenChange,
  } = props
  const [internalOpen, setInternalOpen] = useState(false)
  const [isLoading, setIsLoading] = useState(false)

  const open = controlledOpen !== undefined ? controlledOpen : internalOpen
  const setOpen = onOpenChange || setInternalOpen

  const handleConfirm = async () => {
    setIsLoading(true)
    try {
      await onConfirm()
      setOpen(false)
    } finally {
      setIsLoading(false)
    }
  }

  const handleCancel = () => {
    setOpen(false)
    onCancel?.()
  }

  const defaultTrigger =
    isDangerous && !trigger ? (
      <Button
        variant={triggerVariant as any}
        size={triggerSize as any}
        className={isDangerous ? 'text-red-600 hover:text-red-700 dark:text-red-400 dark:hover:text-red-300' : ''}
      >
        <Trash2 className="w-4 h-4" />
        {triggerText}
      </Button>
    ) : (
      trigger || <Button variant={triggerVariant as any} size={triggerSize as any}>{triggerText}</Button>
    )

  return (
    <AlertDialog open={open} onOpenChange={setOpen}>
      <AlertDialogTrigger asChild>
        {defaultTrigger}
      </AlertDialogTrigger>
      <AlertDialogContent>
        <AlertDialogHeader>
          <div className="flex items-start gap-3">
            {isDangerous && (
              <div className="flex-shrink-0 flex items-center justify-center h-10 w-10 rounded-lg bg-red-100 dark:bg-red-900/30">
                <AlertTriangle className="h-5 w-5 text-red-600 dark:text-red-400" />
              </div>
            )}
            <div className="flex-1 min-w-0">
              <AlertDialogTitle className={isDangerous ? 'text-red-700 dark:text-red-400' : ''}>
                {title}
              </AlertDialogTitle>
              <AlertDialogDescription className="mt-2">
                {description}
              </AlertDialogDescription>
            </div>
          </div>
        </AlertDialogHeader>

        {children && <div className="py-4">{children}</div>}

        <AlertDialogFooter>
          <AlertDialogCancel onClick={handleCancel}>
            {cancelLabel}
          </AlertDialogCancel>
          <AlertDialogAction
            onClick={handleConfirm}
            disabled={isLoading}
            className={isDangerous ? 'bg-red-600 hover:bg-red-500 text-white' : ''}
          >
            {isLoading ? (
              <>
                <span className="inline-block animate-spin-smooth">⏳</span>
                {actionLabel}
              </>
            ) : (
              actionLabel
            )}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  )
}

interface useConfirmDialogReturn {
  ConfirmDialog: (props: Omit<ConfirmDialogProps, 'open' | 'onOpenChange'>) => React.ReactNode
  open: boolean
  setOpen: (open: boolean) => void
}

export function useConfirmDialog(): useConfirmDialogReturn {
  const [open, setOpen] = useState(false)

  const ConfirmDialogComponent = (props: Omit<ConfirmDialogProps, 'open' | 'onOpenChange'>) => (
    <ConfirmDialog {...props} open={open} onOpenChange={setOpen} />
  )

  return { ConfirmDialog: ConfirmDialogComponent, open, setOpen }
}
