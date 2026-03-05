import React from 'react';
import Badge from './Badge';

type BadgeVariant = 'default' | 'success' | 'warning' | 'danger' | 'info';

interface Props {
  code?: string;
  id?: number;
  label?: string;
  className?: string;
  /**
   * Kind lets pages request a domain-specific mapping. Examples: 'flat', 'payment', 'billing'.
   * If omitted, a generic (flat-oriented) mapping is used for backwards-compatibility.
   */
  kind?: string;
  /**
   * Optional custom mapping for statuses. Keys should be the lowercase status code or id
   * (string). Values choose the badge variant and optional fallback text.
   * Example: { paid: { variant: 'success', text: 'Paid' } }
   */
  map?: Record<string, { variant: BadgeVariant; text?: string }>;
}

export default function StatusBadge({ code, id, label, className, kind = 'flat', map }: Props) {
  const keyFor = (c?: string | number) => {
    if (c == null) return '';
    // Normalize: lowercase, trim, replace spaces/hyphens and non-alphanum with underscores
    return String(c)
      .toLowerCase()
      .trim()
      .replace(/\s+/g, '_')
      .replace(/-/g, '_')
      .replace(/[^a-z0-9_]/g, '_');
  };

  // Default mappings per kind. Keep these small and easy to extend.
  const defaultMaps: Record<string, Record<string, { variant: BadgeVariant; text?: string }>> = {
    flat: {
      owner_occupied: { variant: 'success', text: 'Active' },
      active: { variant: 'success', text: 'Active' },
      rented: { variant: 'warning', text: 'Rented' },
      vacant: { variant: 'default', text: 'Vacant' },
    },
    payment: {
      paid: { variant: 'success', text: 'Paid' },
      pending: { variant: 'info', text: 'Pending' },
      failed: { variant: 'danger', text: 'Failed' },
      overdue: { variant: 'danger', text: 'Overdue' },
      refunded: { variant: 'info', text: 'Refunded' },
    },
    billing: {
      draft: { variant: 'default', text: 'Draft' },
      sent: { variant: 'info', text: 'Sent' },
      paid: { variant: 'success', text: 'Paid' },
      overdue: { variant: 'danger', text: 'Overdue' },
    },
  };

  const resolvedMap = { ...(defaultMaps[kind] ?? {}), ...(map ?? {}) };

  const lookup = (c?: string | number) => {
    const key = keyFor(c);
    if (!key) return { variant: 'default' as BadgeVariant, text: label ?? '' };
    const mapped = resolvedMap[key];
    if (mapped) return { variant: mapped.variant, text: label ?? mapped.text ?? String(c) };

    if (key === 'active' || key === 'paid' || key === 'owner_occupied') return { variant: 'success' as BadgeVariant, text: label ?? String(c) };
    if (key === 'rented' || key === 'pending') return { variant: 'warning' as BadgeVariant, text: label ?? String(c) };
    if (key === 'failed' || key === 'overdue') return { variant: 'danger' as BadgeVariant, text: label ?? String(c) };
    return { variant: 'default' as BadgeVariant, text: label ?? String(c) };
  };

  const { variant, text } = lookup(label ?? code ?? id);

  // For status badges we want stronger contrast and a slightly larger pill.
  // Provide override classes that will come after the Badge's variant classes
  // so they take precedence. Consumers can still pass `className` to tweak.
  const variantOverride: Record<BadgeVariant, string> = {
    default: 'bg-gray-600 text-white',
    success: 'bg-green-600 text-white',
    warning: 'bg-yellow-500 text-white',
    danger: 'bg-red-600 text-white',
    info: 'bg-blue-600 text-white',
  };

  const extra = `${variantOverride[variant]} px-3 py-1 text-sm font-semibold rounded-full ${className ?? ''}`.trim();

  return (
    <Badge variant={variant} className={extra} aria-label={`status ${text}`}>
      {text}
    </Badge>
  );
}
