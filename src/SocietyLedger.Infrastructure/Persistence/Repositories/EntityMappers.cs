using System;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Infrastructure.Persistence.Entities;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    internal static class EntityMappers
    {
        // ============================
        // User
        // ============================

        public static User? ToDomain(this user? src)
        {
            if (src == null) return null;

            return new User
            {
                Id = src.id,
                PublicId = src.public_id == Guid.Empty ? Guid.NewGuid() : src.public_id,
                SocietyId = src.society_id,
                SocietyPublicId = src.society?.public_id ?? Guid.Empty,
                Name = src.name ?? string.Empty,
                Email = src.email ?? string.Empty,
                Username = src.username ?? string.Empty,
                Mobile = src.mobile ?? string.Empty,
                RoleId = src.role_id,
                Role = src.role?.ToDomain(),
                PasswordHash = src.password_hash ?? string.Empty,
                IsActive = src.is_active,
                ForcePasswordChange = src.force_password_change,
                LastLogin = src.last_login,
                CreatedAt = src.created_at,
                UpdatedAt = src.updated_at
            };
        }

        public static user ToEntity(this User src)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));

            return new user
            {
                id = src.Id,
                public_id = src.PublicId == Guid.Empty ? Guid.NewGuid() : src.PublicId,
                society_id = src.SocietyId,
                name = src.Name,
                email = src.Email,
                username = src.Username,
                mobile = src.Mobile,
                role_id = src.RoleId,
                password_hash = src.PasswordHash,
                is_active = src.IsActive,
                force_password_change = src.ForcePasswordChange,
                last_login = src.LastLogin,
                created_at = src.CreatedAt,
                updated_at = src.UpdatedAt
            };
        }

        public static void ApplyTo(this User src, user target)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));
            if (target == null) throw new ArgumentNullException(nameof(target));

            target.name = src.Name;
            target.email = src.Email;
            target.mobile = src.Mobile;
            target.role_id = src.RoleId;
            target.password_hash = src.PasswordHash;
            target.is_active = src.IsActive;
            target.force_password_change = src.ForcePasswordChange;
            target.last_login = src.LastLogin;
            target.updated_at = src.UpdatedAt;
        }

        // ============================
        // Society
        // ============================

        public static Society? ToDomain(this society? src)
        {
            if (src == null) return null;

            var society = Society.Create(
                name: src.name ?? string.Empty,
                address: src.address,
                city: src.city,
                state: src.state,
                country: null,
                pincode: src.pincode
            );

            // Use reflection to set protected properties from BaseEntity
            var setId = typeof(BaseEntity).GetMethod("SetId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            setId?.Invoke(society, new object[] { src.id });

            var setPublicId = typeof(BaseEntity).GetMethod("SetPublicId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            setPublicId?.Invoke(society, new object[] { src.public_id == Guid.Empty ? Guid.NewGuid() : src.public_id });

            return society;
        }

        public static society ToEntity(this Society src)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));

            return new society
            {
                id = src.Id,
                public_id = src.PublicId == Guid.Empty ? Guid.NewGuid() : src.PublicId,
                name = src.Name,
                address = src.Address,
                //is_active = src.IsActive,
                created_at = src.CreatedAt,
                updated_at = src.UpdatedAt
            };
        }

        // ============================
        // Role
        // ============================

        public static Role? ToDomain(this role? src)
        {
            if (src == null) return null;

            return new Role
            {
                Id = src.id,
                Code = src.code ?? string.Empty,
                DisplayName = src.display_name ?? string.Empty
            };
        }

        public static role ToEntity(this Role src)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));

            return new role
            {
                id = src.Id,
                code = src.Code,
                display_name = src.DisplayName
            };
        }

        public static Flat ToDomain(this flat entity)
        {
            return new Flat
            {
                Id = entity.id,
                PublicId = entity.public_id,
                SocietyId = entity.society_id,
                SocietyPublicId = entity.society?.public_id ?? Guid.Empty,
                FlatNo = entity.flat_no,
                OwnerName = entity.owner_name,
                ContactMobile = entity.contact_mobile,
                ContactEmail = entity.contact_email,
                MaintenanceAmount = entity.maintenance_amount,
                StatusId = entity.status_id,
                StatusName = entity.status?.display_name ?? string.Empty,
                CreatedAt = entity.created_at,
                UpdatedAt = entity.updated_at
            };
        }

        public static flat ToEntity(this Flat domain)
        {
            return new flat
            {

                id = domain.Id,
                public_id = domain.PublicId,
                society_id = domain.SocietyId,
                flat_no = domain.FlatNo,
                owner_name = domain.OwnerName,
                contact_mobile = domain.ContactMobile,
                contact_email = domain.ContactEmail,
                maintenance_amount = domain.MaintenanceAmount,
                status_id = domain.StatusId,
                created_at = domain.CreatedAt,
                updated_at = domain.UpdatedAt
            };
        }


        public static FlatStatus ToDomain(this flat_status e)
        {
            return new FlatStatus
            {
                Id = (short)e.id,
                Code = e.code,
                DisplayName = e.display_name
            };
        }

        public static flat_status ToEntity(this FlatStatus d)
        {
            return new flat_status
            {
                id = d.Id,
                code = d.Code,
                display_name = d.DisplayName
            };
        }

        // ============================
        // Subscription
        // ============================

        public static Subscription? ToDomain(this subscription? src)
        {
            if (src == null) return null;

            return new Subscription
            {
                Id = src.id,
                UserId = src.user_id,
                PlanId = src.plan_id,
                Status = src.status,
                SubscribedAmount = src.subscribed_amount,
                Currency = src.currency,
                CurrentPeriodStart = src.current_period_start,
                CurrentPeriodEnd = src.current_period_end,
                TrialStart = src.trial_start,
                TrialEnd = src.trial_end,
                CancelledAt = src.cancelled_at,
                CreatedAt = src.created_at,
                UpdatedAt = src.updated_at,
                Plan = src.plan?.ToDomain()
            };
        }

        public static subscription ToEntity(this Subscription src)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));

            return new subscription
            {
                id = src.Id,
                user_id = src.UserId,
                plan_id = src.PlanId,
                status = src.Status,
                subscribed_amount = src.SubscribedAmount,
                currency = src.Currency,
                current_period_start = src.CurrentPeriodStart,
                current_period_end = src.CurrentPeriodEnd,
                trial_start = src.TrialStart,
                trial_end = src.TrialEnd,
                cancelled_at = src.CancelledAt,
                created_at = src.CreatedAt,
                updated_at = src.UpdatedAt
            };
        }

        // ============================
        // Plan
        // ============================

        public static Plan? ToDomain(this plan? src)
        {
            if (src == null) return null;

            return new Plan
            {
                Id = src.id,
                Name = src.name,
                MonthlyAmount = src.monthly_amount,
                Currency = src.currency,
                IsActive = src.is_active,
                CreatedAt = src.created_at
            };
        }

        public static plan ToEntity(this Plan src)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));

            return new plan
            {
                id = src.Id,
                name = src.Name,
                monthly_amount = src.MonthlyAmount,
                currency = src.Currency,
                is_active = src.IsActive,
                created_at = src.CreatedAt
            };
        }

        // ============================
        // Invoice
        // ============================

        public static Invoice? ToDomain(this invoice? src)
        {
            if (src == null) return null;

            return new Invoice
            {
                Id = src.id,
                UserId = src.user_id,
                SubscriptionId = src.subscription_id,
                InvoiceNumber = src.invoice_number,
                InvoiceType = src.invoice_type,
                Amount = src.amount,
                TaxAmount = src.tax_amount,
                TotalAmount = src.total_amount,
                Currency = src.currency,
                Status = src.status,
                PeriodStart = src.period_start,
                PeriodEnd = src.period_end,
                DueDate = src.due_date,
                PaidDate = src.paid_date,
                PaymentMethod = src.payment_method,
                PaymentReference = src.payment_reference,
                Description = src.description,
                CreatedAt = src.created_at,
                UpdatedAt = src.updated_at,
                Subscription = src.subscription?.ToDomain()
            };
        }

        public static invoice ToEntity(this Invoice src)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));

            return new invoice
            {
                id = src.Id,
                user_id = src.UserId,
                subscription_id = src.SubscriptionId,
                invoice_number = src.InvoiceNumber,
                invoice_type = src.InvoiceType,
                amount = src.Amount,
                tax_amount = src.TaxAmount,
                total_amount = src.TotalAmount,
                currency = src.Currency,
                status = src.Status,
                period_start = src.PeriodStart,
                period_end = src.PeriodEnd,
                due_date = src.DueDate,
                paid_date = src.PaidDate,
                payment_method = src.PaymentMethod,
                payment_reference = src.PaymentReference,
                description = src.Description,
                created_at = src.CreatedAt,
                updated_at = src.UpdatedAt
            };
        }

        // ============================
        // SubscriptionEvent
        // ============================

        public static SubscriptionEvent? ToDomain(this subscription_event? src)
        {
            if (src == null) return null;

            return new SubscriptionEvent
            {
                Id = src.id,
                UserId = src.user_id,
                SubscriptionId = src.subscription_id,
                EventType = src.event_type,
                OldStatus = src.old_status,
                NewStatus = src.new_status,
                Amount = src.amount,
                Metadata = src.metadata,
                CreatedAt = src.created_at,
                Subscription = src.subscription?.ToDomain()
            };
        }

        public static subscription_event ToEntity(this SubscriptionEvent src)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));

            return new subscription_event
            {
                id = src.Id,
                user_id = src.UserId,
                subscription_id = src.SubscriptionId,
                event_type = src.EventType,
                old_status = src.OldStatus,
                new_status = src.NewStatus,
                amount = src.Amount,
                metadata = src.Metadata,
                created_at = src.CreatedAt
            };
        }
        public static Payment? ToDomain(this payment? src)
        {
            if (src == null) return null;

            return new Payment
            {
                Id = src.id,
                PublicId = src.public_id,
                SocietyId = src.society_id,
                BillId = src.bill_id,
                FlatId = src.flat_id,
                Amount = src.amount,
                DatePaid = src.date_paid,
                ModeCode = src.mode_code,
                Reference = src.reference,
                ReceiptUrl = src.receipt_url,
                RecordedBy = src.recorded_by,
                //IdempotencyKey = src.idempotency_key,
                //ReversedByPaymentId = src.reversed_by_payment_id,
                //CreatedAt = src.created_at,
                RazorpayOrderId = src.razorpay_order_id,
                RazorpayPaymentId = src.razorpay_payment_id,
                RazorpaySignature = src.razorpay_signature,
                PaymentType = src.payment_type,
                VerifiedAt = src.verified_at
            };
        }

        public static payment ToEntity(this Payment domain)
        {
            return new payment
            {
                id = domain.Id,
                public_id = domain.PublicId,
                society_id = domain.SocietyId,
                bill_id = domain.BillId,
                flat_id = domain.FlatId,
                amount = domain.Amount,
                date_paid = domain.DatePaid,
                mode_code = domain.ModeCode,
                reference = domain.Reference,
                receipt_url = domain.ReceiptUrl,
                recorded_by = domain.RecordedBy,
                idempotency_key = domain.IdempotencyKey,
                reversed_by_payment_id = domain.ReversedByPaymentId,
                created_at = domain.CreatedAt,
                razorpay_order_id = domain.RazorpayOrderId,
                razorpay_payment_id = domain.RazorpayPaymentId,
                razorpay_signature = domain.RazorpaySignature,
                payment_type = domain.PaymentType,
                verified_at = domain.VerifiedAt
            };
        }
    }
}
