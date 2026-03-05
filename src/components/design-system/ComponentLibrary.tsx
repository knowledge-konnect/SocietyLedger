"use client"

import React from "react"

/**
 * Reusable Component Library based on Design System
 * Export these components for consistent implementation across all pages
 */

// ============ Button Component ============
export interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: "primary" | "secondary" | "outline" | "ghost"
  size?: "sm" | "md" | "lg"
  icon?: React.ReactNode
  isLoading?: boolean
}

export const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  ({ variant = "primary", size = "md", icon, isLoading, children, className = "", ...props }, ref) => {
    const baseStyles =
      "font-semibold rounded-lg transition-all duration-200 flex items-center justify-center gap-2 micro-interaction"

    const variants = {
      primary: "bg-primary text-white shadow-base hover:shadow-md hover:-translate-y-1",
      secondary: "bg-secondary text-white shadow-base hover:shadow-md hover:-translate-y-1",
      outline: "border-2 border-primary text-primary hover:bg-primary-light",
      ghost: "text-primary hover:bg-primary-light",
    }

    const sizes = {
      sm: "px-4 py-2 text-sm",
      md: "px-6 py-3 text-base",
      lg: "px-8 py-4 text-lg",
    }

    return (
      <button
        ref={ref}
        className={`${baseStyles} ${variants[variant]} ${sizes[size]} ${className}`}
        disabled={isLoading}
        {...props}
      >
        {isLoading ? (
          <div className="w-5 h-5 border-2 border-current border-r-transparent rounded-full animate-spin" />
        ) : (
          icon
        )}
        {children}
      </button>
    )
  },
)

Button.displayName = "Button"

// ============ Card Component ============
export interface CardProps extends React.HTMLAttributes<HTMLDivElement> {
  hover?: boolean
  gradient?: boolean
}

export const Card = React.forwardRef<HTMLDivElement, CardProps>(
  ({ hover = false, gradient = false, className = "", children, ...props }, ref) => {
    const baseStyles = "rounded-lg border border-border/50 p-6 md:p-8 shadow-base"
    const hoverStyles = hover ? "card-hover hover:shadow-md" : ""
    const gradientStyles = gradient ? "gradient-subtle" : "bg-card"

    return (
      <div ref={ref} className={`${baseStyles} ${hoverStyles} ${gradientStyles} ${className}`} {...props}>
        {children}
      </div>
    )
  },
)

Card.displayName = "Card"

// ============ Badge Component ============
export interface BadgeProps extends React.HTMLAttributes<HTMLSpanElement> {
  variant?: "default" | "success" | "warning" | "error" | "info"
}

export const Badge = React.forwardRef<HTMLSpanElement, BadgeProps>(
  ({ variant = "default", className = "", children, ...props }, ref) => {
    const variants = {
      default: "bg-primary-light text-primary border border-primary-muted",
      success:
        "bg-green-100 text-green-700 dark:bg-green-950/30 dark:text-green-400 border border-green-200 dark:border-green-800",
      warning:
        "bg-amber-100 text-amber-700 dark:bg-amber-950/30 dark:text-amber-400 border border-amber-200 dark:border-amber-800",
      error: "bg-red-100 text-red-700 dark:bg-red-950/30 dark:text-red-400 border border-red-200 dark:border-red-800",
      info: "bg-blue-100 text-blue-700 dark:bg-blue-950/30 dark:text-blue-400 border border-blue-200 dark:border-blue-800",
    }

    return (
      <span
        ref={ref}
        className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold ${variants[variant]} ${className}`}
        {...props}
      >
        {children}
      </span>
    )
  },
)

Badge.displayName = "Badge"

// ============ Feature Card Component ============
export interface FeatureCardProps {
  icon: React.ReactNode
  title: string
  description: string
  onClick?: () => void
}

export const FeatureCard: React.FC<FeatureCardProps> = ({ icon, title, description, onClick }) => (
  <Card hover onClick={onClick} className={onClick ? "cursor-pointer" : ""}>
    <div className="w-12 h-12 bg-gradient-to-br from-primary to-secondary rounded-lg flex items-center justify-center mb-4">
      <div className="text-white">{icon}</div>
    </div>
    <h3 className="text-lg font-bold text-foreground mb-2">{title}</h3>
    <p className="text-muted-foreground leading-relaxed">{description}</p>
  </Card>
)

// ============ Stat Card Component ============
export interface StatCardProps {
  title: string
  value: string | number
  change?: number
  icon: React.ReactNode
}

export const StatCard: React.FC<StatCardProps> = ({ title, value, change, icon }) => (
  <Card>
    <div className="flex items-start justify-between mb-4">
      <div className="w-12 h-12 bg-gradient-to-br from-primary to-secondary rounded-lg flex items-center justify-center text-white">
        {icon}
      </div>
      {change !== undefined && (
        <div
          className={`text-sm font-semibold flex items-center gap-1 ${change > 0 ? "text-green-600" : "text-red-600"}`}
        >
          {change > 0 ? "↑" : "↓"} {Math.abs(change)}%
        </div>
      )}
    </div>
    <p className="text-sm text-muted-foreground mb-1">{title}</p>
    <p className="text-3xl font-bold text-foreground">{value}</p>
  </Card>
)

// ============ Input Component ============
export interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string
  error?: string
  icon?: React.ReactNode
}

export const Input = React.forwardRef<HTMLInputElement, InputProps>(
  ({ label, error, icon, className = "", ...props }, ref) => (
    <div className="space-y-2">
      {label && <label className="block text-sm font-semibold text-foreground">{label}</label>}
      <div className="relative">
        {icon && <div className="absolute left-4 top-3.5 text-muted-foreground">{icon}</div>}
        <input
          ref={ref}
          className={`w-full ${icon ? "pl-12" : "px-4"} py-3 border border-border rounded-lg bg-background focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all ${className}`}
          {...props}
        />
      </div>
      {error && <p className="text-sm text-error">{error}</p>}
    </div>
  ),
)

Input.displayName = "Input"

// ============ Progress Bar Component ============
export interface ProgressProps {
  value: number
  max?: number
  label?: string
  variant?: "primary" | "success" | "warning" | "error"
}

export const Progress: React.FC<ProgressProps> = ({ value, max = 100, label, variant = "primary" }) => {
  const percentage = (value / max) * 100
  const variantColors = {
    primary: "bg-primary",
    success: "bg-green-600",
    warning: "bg-amber-600",
    error: "bg-red-600",
  }

  return (
    <div className="space-y-2">
      {label && (
        <div className="flex justify-between text-sm font-medium">
          <span className="text-foreground">{label}</span>
          <span className="text-muted-foreground">{percentage}%</span>
        </div>
      )}
      <div className="h-2 bg-border rounded-full overflow-hidden">
        <div
          className={`h-full ${variantColors[variant]} transition-all duration-300`}
          style={{ width: `${percentage}%` }}
        />
      </div>
    </div>
  )
}

// ============ Alert Component ============
export interface AlertProps extends React.HTMLAttributes<HTMLDivElement> {
  variant?: "info" | "success" | "warning" | "error"
  title?: string
  icon?: React.ReactNode
}

export const Alert = React.forwardRef<HTMLDivElement, AlertProps>(
  ({ variant = "info", title, icon, children, className = "", ...props }, ref) => {
    const variants = {
      info: "bg-blue-50 dark:bg-blue-950/30 border border-blue-200 dark:border-blue-800 text-blue-900 dark:text-blue-200",
      success:
        "bg-green-50 dark:bg-green-950/30 border border-green-200 dark:border-green-800 text-green-900 dark:text-green-200",
      warning:
        "bg-amber-50 dark:bg-amber-950/30 border border-amber-200 dark:border-amber-800 text-amber-900 dark:text-amber-200",
      error: "bg-red-50 dark:bg-red-950/30 border border-red-200 dark:border-red-800 text-red-900 dark:text-red-200",
    }

    return (
      <div ref={ref} className={`p-4 rounded-lg ${variants[variant]} ${className}`} {...props}>
        <div className="flex gap-3">
          {icon && <div className="flex-shrink-0">{icon}</div>}
          <div className="flex-1">
            {title && <h4 className="font-semibold mb-1">{title}</h4>}
            {children}
          </div>
        </div>
      </div>
    )
  },
)

Alert.displayName = "Alert"
