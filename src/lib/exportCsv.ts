export function exportCsv(rows: any[], headers: string[], filename = 'export.csv') {
  if (!rows || rows.length === 0) return;

  const csvRows = [headers.join(',')];
  for (const r of rows) {
    const values = headers.map(h => r[h] ?? r[h.toLowerCase()] ?? '');
    const escaped = values.map(v => `"${String(v ?? '').replace(/"/g, '""')}"`).join(',');
    csvRows.push(escaped);
  }

  const csv = csvRows.join('\r\n');
  const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = filename;
  document.body.appendChild(a);
  a.click();
  a.remove();
  URL.revokeObjectURL(url);
}
