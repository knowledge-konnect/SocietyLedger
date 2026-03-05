using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SocietyLedger.Infrastructure.Persistence.Entities;

namespace SocietyLedger.Infrastructure.Persistence.Contexts;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<_lock> locks { get; set; }

    public virtual DbSet<adjustment> adjustments { get; set; }

    public virtual DbSet<aggregatedcounter> aggregatedcounters { get; set; }

    public virtual DbSet<attachment> attachments { get; set; }

    public virtual DbSet<audit_log> audit_logs { get; set; }

    public virtual DbSet<bill> bills { get; set; }

    public virtual DbSet<bill_item> bill_items { get; set; }

    public virtual DbSet<bill_payment_allocation> bill_payment_allocations { get; set; }

    public virtual DbSet<bill_status> bill_statuses { get; set; }

    public virtual DbSet<counter> counters { get; set; }

    public virtual DbSet<expense> expenses { get; set; }

    public virtual DbSet<expense_category> expense_categories { get; set; }

    public virtual DbSet<flat> flats { get; set; }

    public virtual DbSet<flat_status> flat_statuses { get; set; }

    public virtual DbSet<hash> hashes { get; set; }

    public virtual DbSet<invoice> invoices { get; set; }

    public virtual DbSet<job> jobs { get; set; }

    public virtual DbSet<job1> jobs1 { get; set; }

    public virtual DbSet<jobparameter> jobparameters { get; set; }

    public virtual DbSet<jobqueue> jobqueues { get; set; }

    public virtual DbSet<list> lists { get; set; }

    public virtual DbSet<maintenance_component> maintenance_components { get; set; }

    public virtual DbSet<maintenance_config> maintenance_configs { get; set; }

    public virtual DbSet<maintenance_cycle> maintenance_cycles { get; set; }

    public virtual DbSet<maintenance_payment> maintenance_payments { get; set; }

    public virtual DbSet<maintenance_plan> maintenance_plans { get; set; }

    public virtual DbSet<maintenance_rate_history> maintenance_rate_histories { get; set; }

    public virtual DbSet<notification_preference> notification_preferences { get; set; }

    public virtual DbSet<payment> payments { get; set; }

    public virtual DbSet<payment_mode> payment_modes { get; set; }

    public virtual DbSet<plan> plans { get; set; }

    public virtual DbSet<plan_component> plan_components { get; set; }

    public virtual DbSet<refresh_token> refresh_tokens { get; set; }

    public virtual DbSet<role> roles { get; set; }

    public virtual DbSet<schema> schemas { get; set; }

    public virtual DbSet<server> servers { get; set; }

    public virtual DbSet<set> sets { get; set; }

    public virtual DbSet<society> societies { get; set; }

    public virtual DbSet<society_fund_ledger> society_fund_ledgers { get; set; }

    public virtual DbSet<state> states { get; set; }

    public virtual DbSet<subscription> subscriptions { get; set; }

    public virtual DbSet<subscription_event> subscription_events { get; set; }

    public virtual DbSet<user> users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<_lock>(entity =>
        {
            entity.Property(e => e.updatecount).HasDefaultValue(0);
        });

        modelBuilder.Entity<adjustment>(entity =>
        {
            entity.HasKey(e => e.id).HasName("adjustments_pkey");

            entity.HasIndex(e => e.society_id, "idx_adjustments_active").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => new { e.flat_id, e.society_id, e.entry_type, e.remaining_amount }, "idx_adjustments_fifo").HasFilter("((remaining_amount > (0)::numeric) AND (is_deleted = false))");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.entry_type).HasDefaultValueSql("'manual'::text");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.adjustments).HasConstraintName("adjustments_created_by_fkey");

            entity.HasOne(d => d.flat).WithMany(p => p.adjustments).HasConstraintName("adjustments_flat_id_fkey");

            entity.HasOne(d => d.society).WithMany(p => p.adjustments).HasConstraintName("adjustments_society_id_fkey");
        });

        modelBuilder.Entity<aggregatedcounter>(entity =>
        {
            entity.HasKey(e => e.id).HasName("aggregatedcounter_pkey");
        });

        modelBuilder.Entity<attachment>(entity =>
        {
            entity.HasKey(e => e.id).HasName("attachments_pkey");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.society).WithMany(p => p.attachments).HasConstraintName("attachments_society_id_fkey");

            entity.HasOne(d => d.uploaded_byNavigation).WithMany(p => p.attachments).HasConstraintName("attachments_uploaded_by_fkey");
        });

        modelBuilder.Entity<audit_log>(entity =>
        {
            entity.HasKey(e => e.id).HasName("audit_logs_pkey");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
            entity.Property(e => e.changed_at).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<bill>(entity =>
        {
            entity.HasKey(e => e.id).HasName("bills_pkey");

            entity.HasIndex(e => e.society_id, "idx_bills_active").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => new { e.flat_id, e.society_id, e.status_code, e.period }, "idx_bills_unpaid_lookup").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => new { e.society_id, e.flat_id, e.period }, "ux_bill_unique_period")
                .IsUnique()
                .HasFilter("(is_deleted = false)");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
            entity.Property(e => e.balance_amount).HasComputedColumnSql("(amount - paid_amount)", true);
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.generated_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.paid_amount).HasDefaultValueSql("0");
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.status_code).HasDefaultValueSql("'unpaid'::text");
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.flat).WithMany(p => p.bills).HasConstraintName("bills_flat_id_fkey");

            entity.HasOne(d => d.generated_byNavigation).WithMany(p => p.bills).HasConstraintName("bills_generated_by_fkey");

            entity.HasOne(d => d.maintenance_plan).WithMany(p => p.bills).HasConstraintName("fk_bill_plan");

            entity.HasOne(d => d.society).WithMany(p => p.bills).HasConstraintName("bills_society_id_fkey");
        });

        modelBuilder.Entity<bill_item>(entity =>
        {
            entity.HasKey(e => e.id).HasName("bill_items_pkey");

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.bill).WithMany(p => p.bill_items).HasConstraintName("bill_items_bill_id_fkey");
        });

        modelBuilder.Entity<bill_payment_allocation>(entity =>
        {
            entity.HasKey(e => e.id).HasName("bill_payment_allocations_pkey");

            entity.HasOne(d => d.bill).WithMany(p => p.bill_payment_allocations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bill_payment_allocations_bill_id_fkey");

            entity.HasOne(d => d.payment).WithMany(p => p.bill_payment_allocations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bill_payment_allocations_payment_id_fkey");
        });

        modelBuilder.Entity<bill_status>(entity =>
        {
            entity.HasKey(e => e.id).HasName("bill_statuses_pkey");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<counter>(entity =>
        {
            entity.HasKey(e => e.id).HasName("counter_pkey");
        });

        modelBuilder.Entity<expense>(entity =>
        {
            entity.HasKey(e => e.id).HasName("expenses_pkey");

            entity.HasIndex(e => e.society_id, "idx_expenses_active").HasFilter("(is_deleted = false)");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
            entity.Property(e => e.category_code).HasDefaultValueSql("'others'::text");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.status).HasDefaultValueSql("'recorded'::text");

            entity.HasOne(d => d.approved_byNavigation).WithMany(p => p.expenseapproved_byNavigations).HasConstraintName("expenses_approved_by_fkey");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.expensecreated_byNavigations).HasConstraintName("expenses_created_by_fkey");

            entity.HasOne(d => d.society).WithMany(p => p.expenses).HasConstraintName("expenses_society_id_fkey");
        });

        modelBuilder.Entity<expense_category>(entity =>
        {
            entity.HasKey(e => e.id).HasName("expense_categories_pkey");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<flat>(entity =>
        {
            entity.HasKey(e => e.id).HasName("flats_pkey");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.maintenance_amount).HasDefaultValueSql("0.00");
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.society).WithMany(p => p.flats).HasConstraintName("flats_society_id_fkey");

            entity.HasOne(d => d.status).WithMany(p => p.flats)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("flats_status_id_fkey");
        });

        modelBuilder.Entity<flat_status>(entity =>
        {
            entity.HasKey(e => e.id).HasName("flat_statuses_pkey");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<hash>(entity =>
        {
            entity.HasKey(e => e.id).HasName("hash_pkey");

            entity.Property(e => e.updatecount).HasDefaultValue(0);
        });

        modelBuilder.Entity<invoice>(entity =>
        {
            entity.HasKey(e => e.id).HasName("invoices_pkey");

            entity.Property(e => e.id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.currency).HasDefaultValueSql("'INR'::character varying");
            entity.Property(e => e.invoice_type).HasDefaultValueSql("'subscription'::character varying");
            entity.Property(e => e.status).HasDefaultValueSql("'pending'::character varying");
            entity.Property(e => e.tax_amount).HasDefaultValueSql("0.00");
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.subscription).WithMany(p => p.invoices)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("invoices_subscription_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.invoices).HasConstraintName("invoices_user_id_fkey");
        });

        modelBuilder.Entity<job>(entity =>
        {
            entity.HasKey(e => e.id).HasName("job_pkey");

            entity.HasIndex(e => e.statename, "ix_hangfire_job_statename_is_not_null").HasFilter("(statename IS NOT NULL)");

            entity.Property(e => e.updatecount).HasDefaultValue(0);
        });

        modelBuilder.Entity<job1>(entity =>
        {
            entity.HasKey(e => e.id).HasName("jobs_pkey");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
            entity.Property(e => e.attempts).HasDefaultValue(0);
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.status).HasDefaultValueSql("'queued'::text");
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<jobparameter>(entity =>
        {
            entity.HasKey(e => e.id).HasName("jobparameter_pkey");

            entity.Property(e => e.updatecount).HasDefaultValue(0);

            entity.HasOne(d => d.job).WithMany(p => p.jobparameters).HasConstraintName("jobparameter_jobid_fkey");
        });

        modelBuilder.Entity<jobqueue>(entity =>
        {
            entity.HasKey(e => e.id).HasName("jobqueue_pkey");

            entity.HasIndex(e => new { e.fetchedat, e.queue, e.jobid }, "ix_hangfire_jobqueue_fetchedat_queue_jobid").HasNullSortOrder(new[] { NullSortOrder.NullsFirst, NullSortOrder.NullsLast, NullSortOrder.NullsLast });

            entity.Property(e => e.updatecount).HasDefaultValue(0);
        });

        modelBuilder.Entity<list>(entity =>
        {
            entity.HasKey(e => e.id).HasName("list_pkey");

            entity.Property(e => e.updatecount).HasDefaultValue(0);
        });

        modelBuilder.Entity<maintenance_component>(entity =>
        {
            entity.HasKey(e => e.id).HasName("maintenance_components_pkey");

            entity.HasIndex(e => e.society_id, "idx_maintenance_components_society").HasFilter("(is_deleted = false)");

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.is_mandatory).HasDefaultValue(true);
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.society).WithMany(p => p.maintenance_components).HasConstraintName("maintenance_components_society_id_fkey");
        });

        modelBuilder.Entity<maintenance_config>(entity =>
        {
            entity.HasKey(e => e.id).HasName("maintenance_config_pkey");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.due_day_of_month).HasDefaultValue(1);
            entity.Property(e => e.grace_period_days).HasDefaultValue(0);
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.maintenance_configcreated_byNavigations).HasConstraintName("maintenance_config_created_by_fkey");

            entity.HasOne(d => d.society).WithOne(p => p.maintenance_config).HasConstraintName("maintenance_config_society_id_fkey");

            entity.HasOne(d => d.updated_byNavigation).WithMany(p => p.maintenance_configupdated_byNavigations).HasConstraintName("maintenance_config_updated_by_fkey");
        });

        modelBuilder.Entity<maintenance_cycle>(entity =>
        {
            entity.HasKey(e => e.id).HasName("maintenance_cycles_pkey");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_active).HasDefaultValue(true);
        });

        modelBuilder.Entity<maintenance_payment>(entity =>
        {
            entity.HasKey(e => e.id).HasName("maintenance_payments_pkey");

            entity.HasIndex(e => new { e.society_id, e.payment_date }, "idx_mp_society_date_not_deleted")
                .IsDescending(false, true)
                .HasFilter("(NOT is_deleted)");

            entity.HasIndex(e => new { e.society_id, e.idempotency_key, e.bill_id }, "ux_maintenance_idempotency")
                .IsUnique()
                .HasFilter("(idempotency_key IS NOT NULL)");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.adjustment).WithMany(p => p.maintenance_payments)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_maintenance_adjustment");

            entity.HasOne(d => d.bill).WithMany(p => p.maintenance_payments)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_maintenance_bill");

            entity.HasOne(d => d.flat).WithMany(p => p.maintenance_payments).HasConstraintName("fk_maintenance_flat");

            entity.HasOne(d => d.payment_mode).WithMany(p => p.maintenance_payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_maintenance_payment_mode");

            entity.HasOne(d => d.recorded_byNavigation).WithMany(p => p.maintenance_payments).HasConstraintName("fk_maintenance_recorded_by");

            entity.HasOne(d => d.society).WithMany(p => p.maintenance_payments).HasConstraintName("fk_maintenance_society");
        });

        modelBuilder.Entity<maintenance_plan>(entity =>
        {
            entity.HasKey(e => e.id).HasName("maintenance_plans_pkey");

            entity.HasIndex(e => e.society_id, "idx_maintenance_plans_society").HasFilter("(is_deleted = false)");

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.fixed_amount).HasDefaultValueSql("0");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.rate_per_sqft).HasDefaultValueSql("0");

            entity.HasOne(d => d.society).WithMany(p => p.maintenance_plans).HasConstraintName("fk_maintenance_plan_society");
        });

        modelBuilder.Entity<maintenance_rate_history>(entity =>
        {
            entity.HasKey(e => e.id).HasName("maintenance_rate_history_pkey");

            entity.Property(e => e.changed_at).HasDefaultValueSql("now()");
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.maintenance_plan).WithMany(p => p.maintenance_rate_histories).HasConstraintName("maintenance_rate_history_maintenance_plan_id_fkey");
        });

        modelBuilder.Entity<notification_preference>(entity =>
        {
            entity.HasKey(e => e.id).HasName("notification_preferences_pkey");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
            entity.Property(e => e.bill_generated).HasDefaultValue(true);
            entity.Property(e => e.expense_updates).HasDefaultValue(true);
            entity.Property(e => e.monthly_reports).HasDefaultValue(true);
            entity.Property(e => e.payment_reminders).HasDefaultValue(true);
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.user).WithOne(p => p.notification_preference).HasConstraintName("notification_preferences_user_id_fkey");
        });

        modelBuilder.Entity<payment>(entity =>
        {
            entity.HasKey(e => e.id).HasName("payments_pkey");

            entity.HasIndex(e => e.society_id, "idx_payments_active").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => new { e.society_id, e.idempotency_key }, "idx_payments_idempotency")
                .IsUnique()
                .HasFilter("(idempotency_key IS NOT NULL)");

            entity.HasIndex(e => e.razorpay_payment_id, "ux_payments_razorpay_payment")
                .IsUnique()
                .HasFilter("(razorpay_payment_id IS NOT NULL)");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.bill).WithMany(p => p.payments)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("payments_bill_id_fkey");

            entity.HasOne(d => d.flat).WithMany(p => p.payments)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("payments_flat_id_fkey");

            entity.HasOne(d => d.recorded_byNavigation).WithMany(p => p.payments).HasConstraintName("payments_recorded_by_fkey");

            entity.HasOne(d => d.society).WithMany(p => p.payments).HasConstraintName("payments_society_id_fkey");
        });

        modelBuilder.Entity<payment_mode>(entity =>
        {
            entity.HasKey(e => e.id).HasName("payment_modes_pkey");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<plan>(entity =>
        {
            entity.HasKey(e => e.id).HasName("plans_pkey");

            entity.Property(e => e.id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.currency).HasDefaultValueSql("'INR'::character varying");
            entity.Property(e => e.duration_months).HasDefaultValue(1);
            entity.Property(e => e.is_active).HasDefaultValue(true);
        });

        modelBuilder.Entity<plan_component>(entity =>
        {
            entity.HasKey(e => e.id).HasName("plan_components_pkey");

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.maintenance_component).WithMany(p => p.plan_components).HasConstraintName("plan_components_maintenance_component_id_fkey");

            entity.HasOne(d => d.maintenance_plan).WithMany(p => p.plan_components).HasConstraintName("plan_components_maintenance_plan_id_fkey");
        });

        modelBuilder.Entity<refresh_token>(entity =>
        {
            entity.HasKey(e => e.id).HasName("refresh_tokens_pkey");

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_revoked).HasDefaultValue(false);

            entity.HasOne(d => d.user).WithMany(p => p.refresh_tokens).HasConstraintName("refresh_tokens_user_id_fkey");
        });

        modelBuilder.Entity<role>(entity =>
        {
            entity.HasKey(e => e.id).HasName("roles_pkey");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<schema>(entity =>
        {
            entity.HasKey(e => e.version).HasName("schema_pkey");

            entity.Property(e => e.version).ValueGeneratedNever();
        });

        modelBuilder.Entity<server>(entity =>
        {
            entity.HasKey(e => e.id).HasName("server_pkey");

            entity.Property(e => e.updatecount).HasDefaultValue(0);
        });

        modelBuilder.Entity<set>(entity =>
        {
            entity.HasKey(e => e.id).HasName("set_pkey");

            entity.Property(e => e.updatecount).HasDefaultValue(0);
        });

        modelBuilder.Entity<society>(entity =>
        {
            entity.HasKey(e => e.id).HasName("societies_pkey");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.currency).HasDefaultValueSql("'INR'::text");
            entity.Property(e => e.default_maintenance_cycle).HasDefaultValueSql("'monthly'::text");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.onboarding_date).HasDefaultValueSql("CURRENT_DATE");
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.settings).HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.maintenance_cycle).WithMany(p => p.societies).HasConstraintName("societies_maintenance_cycle_id_fkey");
        });

        modelBuilder.Entity<society_fund_ledger>(entity =>
        {
            entity.HasKey(e => e.id).HasName("society_fund_ledger_pkey");

            entity.HasIndex(e => e.society_id, "uq_society_single_opening")
                .IsUnique()
                .HasFilter("((entry_type = 'opening_fund'::text) AND (COALESCE(is_deleted, false) = false))");

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.transaction_date).HasDefaultValueSql("CURRENT_DATE");
        });

        modelBuilder.Entity<state>(entity =>
        {
            entity.HasKey(e => e.id).HasName("state_pkey");

            entity.Property(e => e.updatecount).HasDefaultValue(0);

            entity.HasOne(d => d.job).WithMany(p => p.states).HasConstraintName("state_jobid_fkey");
        });

        modelBuilder.Entity<subscription>(entity =>
        {
            entity.HasKey(e => e.id).HasName("subscriptions_pkey");

            entity.Property(e => e.id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.currency).HasDefaultValueSql("'INR'::character varying");
            entity.Property(e => e.status).HasDefaultValueSql("'trial'::character varying");
            entity.Property(e => e.trial_end).HasDefaultValueSql("(now() + '30 days'::interval)");
            entity.Property(e => e.trial_start).HasDefaultValueSql("now()");
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.plan).WithMany(p => p.subscriptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("subscriptions_plan_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.subscriptions).HasConstraintName("subscriptions_user_id_fkey");
        });

        modelBuilder.Entity<subscription_event>(entity =>
        {
            entity.HasKey(e => e.id).HasName("subscription_events_pkey");

            entity.Property(e => e.id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.subscription).WithMany(p => p.subscription_events)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("subscription_events_subscription_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.subscription_events).HasConstraintName("subscription_events_user_id_fkey");
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.id).HasName("users_pkey");

            entity.Property(e => e.id).UseIdentityAlwaysColumn();
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.force_password_change).HasDefaultValue(false);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.is_deleted).HasDefaultValue(false);
            entity.Property(e => e.monthly_amount).HasDefaultValueSql("299.00");
            entity.Property(e => e.public_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.subscription_plan).HasDefaultValueSql("'pro'::character varying");
            entity.Property(e => e.subscription_status).HasDefaultValueSql("'trial'::character varying");
            entity.Property(e => e.trial_ends_date).HasDefaultValueSql("(now() + '30 days'::interval)");
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.role).WithMany(p => p.users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_role_id_fkey");

            entity.HasOne(d => d.society).WithMany(p => p.users).HasConstraintName("users_society_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
