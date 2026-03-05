using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using SocietyLedger.Api.Filters;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Application.Interfaces.Services;
using SocietyLedger.Application.Validators.Auth;
using SocietyLedger.Infrastructure.Data;
using SocietyLedger.Infrastructure.Jobs;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Repositories;
using SocietyLedger.Infrastructure.Services;
using SocietyLedger.Infrastructure.Services.Common;
using SocietyLedger.Shared.Jwt;
namespace SocietyLedger.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

            // Add application-level services (use cases)
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IFlatService, FlatService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IPlanService, PlanService>();
            services.AddScoped<IRazorpayPaymentService, RazorpayPaymentService>();
            services.AddScoped<IMaintenancePaymentService, MaintenancePaymentService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IOpeningBalanceService, OpeningBalanceService>();
            services.AddScoped<IBillingService, BillingService>();
            services.AddScoped<IMaintenanceConfigService, MaintenanceConfigService>();
            services.AddScoped<INotificationPreferenceService, NotificationPreferenceService>();

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddHttpContextAccessor();

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Common infrastructure helpers
            services.AddScoped<IUserContext, UserContext>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IFlatRepository, FlatRepository>();
            services.AddScoped<ISocietyRepository, SocietyRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<ISubscriptionEventRepository, SubscriptionEventRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IMaintenancePaymentRepository, MaintenancePaymentRepository>();
            services.AddScoped<IPaymentModeRepository, PaymentModeRepository>();
            services.AddScoped<IExpenseRepository, ExpenseRepository>();

            services.AddScoped<IMaintenanceConfigRepository, MaintenanceConfigRepository>();
            services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();

            services.AddSingleton<SocietyLedger.Infrastructure.Data.IDbConnectionFactory, SocietyLedger.Infrastructure.Data.DbConnectionFactory>();
            services.AddScoped<IDapperService, DapperService>();

            // Dashboard Services (Dapper-based)
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IDashboardService, DashboardService>();

            // Report Services (Dapper-based)
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IReportService, ReportService>();

            // OPTIONAL: prefer repository for refresh tokens instead of direct DbContext access.
            // Uncomment & implement IRefreshTokenRepository + RefreshTokenRepository if you add it.
            // services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddSingleton<PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddSingleton(typeof(FluentValidationFilter<>));

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            return services;
        }

        public static IServiceCollection AddSharedServices(this IServiceCollection services)
        {
            services.AddLogging();
            services.AddMemoryCache();
            return services;
        }

        /// <summary>
        /// Configures Hangfire using PostgreSQL storage and registers the
        /// <see cref="MonthlyBillingJob"/> into the DI container so that
        /// Hangfire's job activator can resolve it with its dependencies.
        ///
        /// Call <c>UseHangfireServer()</c> and <c>UseHangfireDashboard()</c>
        /// in the middleware pipeline after calling this method.
        /// </summary>
        public static IServiceCollection AddHangfireServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("HangfireConnection")
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Neither 'HangfireConnection' nor 'DefaultConnection' is configured.");

            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(c =>
                    c.UseNpgsqlConnection(connectionString),
                    new PostgreSqlStorageOptions
                    {
                        // Keep job history for 30 days.
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        // Poll interval for background Hangfire server.
                        QueuePollInterval          = TimeSpan.FromSeconds(15)
                    }));

            // Add the Hangfire server that processes background jobs.
            services.AddHangfireServer(options =>
            {
                options.ServerName = $"SocietyLedger:{Environment.MachineName}";
                // Only one worker needed for this app; increase if queue depth grows.
                options.WorkerCount = Math.Max(1, Environment.ProcessorCount / 2);
                options.Queues      = ["default", "billing"];
            });

            // Register the job class so it can be resolved by Hangfire's DI activator.
            services.AddScoped<MonthlyBillingJob>();

            return services;
        }
    }
}
