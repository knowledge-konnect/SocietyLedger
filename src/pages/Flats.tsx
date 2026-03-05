import { useState, useEffect } from 'react';
import { Plus, Search, Download, Edit, Trash, AlertCircle } from 'lucide-react';
import DashboardLayout from '../components/layout/DashboardLayout';
import Card from '../components/ui/Card';
import Button from '../components/ui/Button';
import { Table, TableHeader, TableBody, TableRow, TableHead, TableCell } from '../components/ui/Table';
import StatusBadge from '../components/ui/StatusBadge';
import Pagination from '../components/ui/Pagination';
import { useDebounce } from '../hooks/useDebounce';
import { exportCsv as exportCsvLib } from '../lib/exportCsv';
import { useKeyboardShortcut } from '../hooks/useKeyboardShortcut';
import { useFlatStatuses } from '../hooks/useFlatsApi';
import Modal, { ModalFooter } from '../components/ui/Modal';
import Tooltip from '../components/ui/Tooltip';
import { formatCurrency } from '../lib/utils';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';

const flatSchema = z.object({
  flatNumber: z.string().min(1, 'Flat number is required'),
  ownerName: z.string().min(1, 'Owner name is required'),
  ownerEmail: z.string().optional(),
  ownerPhone: z.string().min(10, 'Phone number must be at least 10 digits'),
  maintenanceAmount: z.string().refine(
    val => !isNaN(Number(val)) && Number(val) > 0,
    'Maintenance amount must be a positive number'
  ),
  statusId: z.string().min(1, 'Status is required'),
});

type FlatFormData = z.infer<typeof flatSchema>;

import { useFlats, useCreateFlat, useUpdateFlat, useDeleteFlat } from '../hooks/useFlatsApi';
import { useAuth } from '../contexts/AuthProvider';
import { useToast } from '../components/ui/Toast';

// We'll map API FlatDto to the UI model used below
type UIFLat = {
  id: string;
  flatNumber: string;
  ownerName: string;
  ownerEmail: string;
  ownerPhone: string;
  maintenanceAmount: number;
  outstandingBalance: number;
  // UI display name for status
  status: string;
  // numeric status id saved in DB
  statusId?: number;
  createdAt?: string;
  publicId?: string;
};

export default function Flats() {
  const [searchQuery, setSearchQuery] = useState('');
  const debouncedSearch = useDebounce(searchQuery, 250);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [sortBy, setSortBy] = useState<'createdAt' | 'flatNumber'>('createdAt');
  const [sortDir, setSortDir] = useState<'desc' | 'asc'>('desc');

  const [showAddModal, setShowAddModal] = useState(false);
  const [selectedFlat, setSelectedFlat] = useState<any>(null);
  const [isEditing, setIsEditing] = useState(false);
  const [flats, setFlats] = useState<UIFLat[]>([]);
  const { user } = useAuth();
  const societyId = user?.societyId ? Number(user.societyId) : undefined;

  const { data: apiFlats, isLoading: apiFlatsLoading, isError: apiFlatsError } = useFlats(societyId);
  const createFlat = useCreateFlat();
  const updateFlat = useUpdateFlat();
  const deleteFlat = useDeleteFlat();
  const { data: statuses } = useFlatStatuses();
  const { showToast } = useToast();
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<UIFLat | null>(null);

  // explicit empty form baseline
  const emptyForm: FlatFormData = {
    flatNumber: '',
    ownerName: '',
    ownerEmail: '',
    ownerPhone: '',
    maintenanceAmount: '',
    statusId: '',
  };

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset,
  } = useForm<FlatFormData>({
    resolver: zodResolver(flatSchema),
    defaultValues: emptyForm,
  });

  // helper to open add modal with a clean form
  const openAddModal = () => {
    setIsEditing(false);
    setSelectedFlat(null);
    reset(emptyForm);
    setShowAddModal(true);
  };

  useKeyboardShortcut('n', () => {
    const tag = (document.activeElement?.tagName || '').toLowerCase();
    if (tag === 'input' || tag === 'textarea') return;
    openAddModal();
  });

  const onSubmit = async (data: FlatFormData) => {
    try {
      if (!societyId) throw new Error('No society selected');

      let selectedStatusId: number | undefined;
      if ((data as any).statusId) {
        selectedStatusId = Number((data as any).statusId);
      } else if (statuses && statuses.length) {
        const def = statuses.find(s => s.code === 'owner_occupied');
        selectedStatusId = def?.id;
      }

      if (isEditing && selectedFlat?.publicId) {
        const payload = {
          publicId: selectedFlat.publicId,
          flatNo: data.flatNumber,
          ownerName: data.ownerName,
          contactMobile: data.ownerPhone,
          contactEmail: data.ownerEmail,
          maintenanceAmount: Number(data.maintenanceAmount),
          statusId: selectedStatusId,
        };
        await updateFlat.mutateAsync(payload as any);
        setShowAddModal(false);
        setIsEditing(false);
        setSelectedFlat(null);
        reset(emptyForm);
        showToast('Flat updated successfully', 'success');
      } else {
        const payload = {
          societyId,
          flatNo: data.flatNumber,
          ownerName: data.ownerName,
          contactMobile: data.ownerPhone,
          contactEmail: data.ownerEmail,
          maintenanceAmount: Number(data.maintenanceAmount),
          statusId: selectedStatusId,
        };
        const created = await createFlat.mutateAsync(payload as any);
        setShowAddModal(false);
        reset(emptyForm);
        if (created && created.publicId) {
          showToast(`Flat ${created.flatNo} created (ID: ${created.publicId})`, 'success');
        } else {
          showToast('Flat created successfully', 'success');
        }
      }
    } catch (error) {
      console.error('Error adding flat:', error);
      showToast('Failed to add flat. Please try again.', 'error');
    }
  };

  // Map API flats to UI model whenever apiFlats or statuses change
  useEffect(() => {
    if (!apiFlats) return;
    const mapped = apiFlats.map((f) => {
      let statusDisplay = '';
      let sid: number | undefined;
      // Prefer server-provided display name
      if ((f as any).statusName) {
        statusDisplay = (f as any).statusName as string;
        sid = (f as any).statusId as number | undefined;
      } else if ((f as any).statusId != null) {
        sid = (f as any).statusId as number;
        statusDisplay = statuses?.find(s => s.id === sid)?.displayName ?? String(sid);
      } else if ((f as any).status) {
        const code = (f as any).status as string;
        statusDisplay = statuses?.find(s => s.code === code)?.displayName ?? code;
      }

      return {
        id: String(f.id),
        publicId: f.publicId,
        flatNumber: f.flatNo,
        ownerName: f.ownerName,
        ownerEmail: f.contactEmail,
        ownerPhone: f.contactMobile,
        maintenanceAmount: f.maintenanceAmount,
        outstandingBalance: 0,
        status: statusDisplay || (f as any).status || '',
        statusId: sid,
        createdAt: (f as any).createdAt,
      } as UIFLat;
    });
    setFlats(mapped);
  }, [apiFlats, statuses]);

  // Filter, sort and paginate
  const processed = flats.filter(flat => {
    const q = debouncedSearch.trim().toLowerCase();
    if (!q) return true;
    return (
      flat.flatNumber.toLowerCase().includes(q) ||
      flat.ownerName.toLowerCase().includes(q) ||
      (flat.ownerEmail || '').toLowerCase().includes(q) ||
      (flat.ownerPhone || '').toLowerCase().includes(q)
    );
  }).sort((a, b) => {
    if (sortBy === 'createdAt') {
      const ta = a.createdAt ? new Date(a.createdAt).getTime() : 0;
      const tb = b.createdAt ? new Date(b.createdAt).getTime() : 0;
      return sortDir === 'asc' ? ta - tb : tb - ta;
    }
    return sortDir === 'asc' ? a.flatNumber.localeCompare(b.flatNumber) : b.flatNumber.localeCompare(a.flatNumber);
  });

  const total = processed.length;
  const start = page * pageSize;
  const paged = processed.slice(start, start + pageSize);

  const exportCsv = () => {
    // Export all currently processed (filtered/sorted/paged) rows
    const rows = processed;
    if (!rows || rows.length === 0) {
      showToast('No flats to export', 'info');
      return;
    }
    const headers = ['Flat Number','Owner Name','Contact Mobile','Contact Email','Maintenance Amount','Status','Public ID','Created At'];
    const rowsData = rows.map(r => ({
      'Flat Number': r.flatNumber,
      'Owner Name': r.ownerName,
      'Contact Mobile': r.ownerPhone,
      'Contact Email': r.ownerEmail,
      'Maintenance Amount': r.maintenanceAmount,
      'Status': r.status,
      'Public ID': r.publicId ?? '',
      'Created At': r.createdAt ?? '',
    }));

    exportCsvLib(rowsData, headers, `flats_${societyId ?? 'all'}_${new Date().toISOString().slice(0,10)}.csv`);
    showToast('CSV exported', 'success');
  };

  return (
    <DashboardLayout title="Flats Management">
      <div className="space-y-6">
        <div className="flex flex-col sm:flex-row gap-3 items-start sm:items-center justify-between">
          <div className="relative flex-1 max-w-md">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
            <input
              type="text"
              placeholder="Search flats..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="input-base pl-11"
            />
          </div>

          <div className="flex gap-2">
            <Button variant="outline" size="sm" onClick={() => exportCsv()} disabled={flats.length === 0}>
              <Download className="w-4 h-4 mr-2" />
              Export
            </Button>
            <Button size="sm" onClick={() => openAddModal()}>
              <Plus className="w-4 h-4 mr-2" />
              Add Flat
            </Button>
          </div>
        </div>

        <div className="p-0">
          {apiFlatsLoading ? (
            <div className="space-y-2">
              {[1,2,3,4,5].map(i => (
                <div key={i} className="w-full h-10 bg-slate-200 dark:bg-slate-700 rounded animate-pulse" />
              ))}
            </div>
          ) : (!flats || flats.length === 0) ? (
            <div className="text-center py-12">
              <h3 className="text-lg font-bold text-foreground">No flats found</h3>
              <p className="text-sm text-muted-foreground mt-2">Add your first flat to get started.</p>
              <div className="mt-4">
                <Button onClick={() => openAddModal()}>Add Flat</Button>
              </div>
            </div>
          ) : (
            <>
              <div className="w-full overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Flat</TableHead>
                    <TableHead>Owner</TableHead>
                    <TableHead className="hidden md:table-cell">Phone</TableHead>
                    <TableHead className="text-right">Maintenance</TableHead>
                    <TableHead className="hidden lg:table-cell text-right">Outstanding</TableHead>
                    <TableHead className="hidden sm:table-cell">Status</TableHead>
                    <TableHead className="text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {paged.map((flat) => (
                    <TableRow key={flat.id}>
                      <TableCell>
                        <span className="font-semibold text-foreground">
                          {flat.flatNumber}
                        </span>
                      </TableCell>
                      <TableCell className="font-medium text-foreground">{flat.ownerName}</TableCell>
                      <TableCell className="hidden md:table-cell text-xs lg:text-sm">
                        <div className="text-foreground">{flat.ownerPhone}</div>
                      </TableCell>
                      <TableCell className="text-right font-medium text-foreground">{formatCurrency(flat.maintenanceAmount)}</TableCell>
                      <TableCell className="hidden lg:table-cell text-right">
                        <span className={flat.outstandingBalance > 0 ? 'text-destructive font-semibold' : 'text-success font-semibold'}>
                          {formatCurrency(flat.outstandingBalance)}
                        </span>
                      </TableCell>
                      <TableCell className="hidden sm:table-cell">
                        <StatusBadge code={flat.status} id={flat.statusId} label={flat.status} kind="flat" />
                      </TableCell>
                      <TableCell className="text-right">
                        <div className="flex gap-1 justify-end">
                          <Tooltip content="Edit" side="top">
                            <Button
                              variant="ghost"
                              size="sm"
                              aria-label={`Edit ${flat.flatNumber}`}
                              onClick={() => {
                                setSelectedFlat(flat);
                                // prefill form
                                reset({
                                  flatNumber: flat.flatNumber,
                                  ownerName: flat.ownerName,
                                  ownerEmail: flat.ownerEmail,
                                  ownerPhone: flat.ownerPhone,
                                  maintenanceAmount: String(flat.maintenanceAmount),
                                  statusId: flat.statusId ? String(flat.statusId) : '',
                                });
                                setIsEditing(true);
                                setShowAddModal(true);
                              }}
                            >
                              <Edit className="w-4 h-4" />
                            </Button>
                          </Tooltip>

                          <Tooltip content="Delete" side="top">
                            <Button
                              variant="ghost"
                              size="sm"
                              aria-label={`Delete ${flat.flatNumber}`}
                              onClick={() => {
                                setDeleteTarget(flat);
                                setShowDeleteModal(true);
                              }}
                            >
                              <Trash className="w-4 h-4 text-red-600" />
                            </Button>
                          </Tooltip>
                        </div>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
              </div>
              <div className="mt-4">
                <Pagination page={page} pageSize={pageSize} total={total} onPageChange={(p) => setPage(p)} onPageSizeChange={(s) => { setPageSize(s); setPage(0); }} />
              </div>
            </>
          )}
        </div>
      </div>

      <Modal
        isOpen={showAddModal}
        onClose={() => {
          setShowAddModal(false);
          reset(emptyForm);
          setIsEditing(false);
          setSelectedFlat(null);
        }}
        title={isEditing ? 'Edit Flat' : 'Add New Flat'}
        size="lg"
      >
        <form onSubmit={handleSubmit(onSubmit)} className="form-group space-y-4 md:space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 md:gap-6">
            <div className="form-field">
              <label className="block text-sm font-semibold text-foreground mb-2">
                Flat Number
              </label>
              <input
                type="text"
                placeholder="e.g., A-101"
                {...register('flatNumber')}
                className="input-base"
              />
              {errors.flatNumber && (
                <div className="flex items-center gap-2 mt-1 text-sm text-destructive font-semibold">
                  <AlertCircle className="w-4 h-4" />
                  {errors.flatNumber.message}
                </div>
              )}
            </div>

            <div className="form-field">
              <label className="block text-sm font-semibold text-foreground mb-2">
                Owner Name
              </label>
              <input
                type="text"
                placeholder="Full name"
                {...register('ownerName')}
                className="input-base"
              />
              {errors.ownerName && (
                <div className="flex items-center gap-2 mt-1 text-sm text-destructive font-semibold">
                  <AlertCircle className="w-4 h-4" />
                  {errors.ownerName.message}
                </div>
              )}
            </div>

            <div className="form-field">
              <label className="block text-sm font-semibold text-foreground mb-2">
                Email <span className="text-muted-foreground text-xs">(optional)</span>
              </label>
              <input
                type="email"
                placeholder="owner@example.com"
                {...register('ownerEmail')}
                className="input-base"
              />
              {errors.ownerEmail && (
                <div className="flex items-center gap-2 mt-1 text-sm text-destructive font-semibold">
                  <AlertCircle className="w-4 h-4" />
                  {errors.ownerEmail.message}
                </div>
              )}
            </div>

            <div className="form-field">
              <label className="block text-sm font-semibold text-foreground mb-2">
                Phone Number
              </label>
              <input
                type="tel"
                maxLength={10}
                minLength={10}
                placeholder="+1234567890"
                {...register('ownerPhone')}
                className="input-base"
              />
              {errors.ownerPhone && (
                <div className="flex items-center gap-2 mt-1 text-sm text-destructive font-semibold">
                  <AlertCircle className="w-4 h-4" />
                  {errors.ownerPhone.message}
                </div>
              )}
            </div>

            <div className="form-field md:col-span-2">
              <label className="block text-sm font-semibold text-foreground mb-2">
                Maintenance Amount
              </label>
              <input
                type="number"
                placeholder="5000"
                step="0.01"
                {...register('maintenanceAmount')}
                className="input-base"
              />
              {errors.maintenanceAmount && (
                <div className="flex items-center gap-2 mt-1 text-sm text-destructive font-semibold">
                  <AlertCircle className="w-4 h-4" />
                  {errors.maintenanceAmount.message}
                </div>
              )}
            </div>

            <div className="form-field md:col-span-2">
              <label className="block text-sm font-semibold text-foreground mb-2">
                Status
              </label>
              <select
                {...register('statusId')}
                className="input-base"
              >
                <option value="">Select Status</option>
                {statuses?.map((s) => (
                  <option key={s.id} value={s.id}>{s.displayName}</option>
                ))}
              </select>
              {errors.statusId && (
                <div className="flex items-center gap-2 mt-1 text-sm text-destructive font-semibold">
                  <AlertCircle className="w-4 h-4" />
                  {errors.statusId.message}
                </div>
              )}
            </div>
          </div>

          <ModalFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => {
                setShowAddModal(false);
                reset(emptyForm);
                setIsEditing(false);
                setSelectedFlat(null);
              }}
            >
              Cancel
            </Button>
            <Button
              type="submit"
              disabled={isSubmitting || (isEditing ? (updateFlat as any).isLoading : (createFlat as any).isLoading)}
            >
              {isEditing
                ? ((updateFlat as any).isLoading ? 'Saving...' : (isSubmitting ? 'Saving...' : 'Save'))
                : ((createFlat as any).isLoading ? 'Adding...' : (isSubmitting ? 'Adding...' : 'Add Flat'))}
            </Button>
          </ModalFooter>
        </form>
      </Modal>

      <Modal
        isOpen={showDeleteModal}
        onClose={() => {
          setShowDeleteModal(false);
          setDeleteTarget(null);
        }}
        title="Delete Flat"
        size="sm"
      >
        <div className="py-4">
          <p className="text-sm text-gray-700 dark:text-gray-300">Are you sure you want to delete <strong>{deleteTarget?.flatNumber}</strong>? This action cannot be undone.</p>
        </div>
        <ModalFooter>
          <Button
            variant="outline"
            type="button"
            onClick={() => {
              setShowDeleteModal(false);
              setDeleteTarget(null);
            }}
          >
            Cancel
          </Button>
          <Button
            type="button"
            variant="danger"
            disabled={(deleteFlat as any).isLoading}
            onClick={async () => {
              if (!deleteTarget) return;
              try {
                await deleteFlat.mutateAsync(deleteTarget.publicId ?? '');
                showToast('Flat deleted', 'success');
                setShowDeleteModal(false);
                setDeleteTarget(null);
              } catch (err) {
                console.error('Delete failed', err);
                showToast('Failed to delete flat', 'error');
              }
            }}
          >
            {(deleteFlat as any).isLoading ? 'Deleting...' : 'Delete'}
          </Button>
        </ModalFooter>
      </Modal>
      {/* Import CSV UI hidden. Export provided above. */}
    </DashboardLayout>
  );
}
