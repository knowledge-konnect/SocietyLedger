using Asp.Versioning;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using SocietyLedger.Api.Authorization;
using SocietyLedger.Api.Endpoints;
using SocietyLedger.Api.Extensions;
using SocietyLedger.Api.Middlewares;
using SocietyLedger.Domain.Exceptions;
using SocietyLedger.Infrastructure.Jobs;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Services;
using SocietyLedger.Shared;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// Serilog configuration
// ----------------------------
LogEventLevel level = LogEventLevel.Information;
var logTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(level)
    .WriteTo.Console(outputTemplate: logTemplate)
    .WriteTo.File("Logs/SocietyLedger-.txt", rollingInterval: RollingInterval.Day, outputTemplate: logTemplate)
    .CreateLogger();

Log.Information("Starting SocietyLedger API...");

// ----------------------------
// CORS
// The frontend sends credentials (withCredentials: true / httpOnly cookie), so
// AllowAnyOrigin() is forbidden by the browser.  Origins are loaded from config.
// ----------------------------
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();   // required for httpOnly cookie
    });
});

// ----------------------------
// Health Checks
// ----------------------------
builder.Services.AddHealthChecks();

// ----------------------------
// Swagger / OpenAPI
// ----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SocietyLedger API",
        Version = "v1"
    });

    // JWT Authorization for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your token.\nExample: \"Bearer abc123\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// ----------------------------
// API Versioning
// ----------------------------
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(ApiConstants.API_VERSION_1_0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ----------------------------
// Rate Limiting
// ----------------------------
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? context.Connection.RemoteIpAddress?.ToString()
                     ?? "anonymous";
        
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: userId,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        var errorResponse = ErrorResponse.Create(
            "RATE_LIMIT_EXCEEDED",
            "Too many requests. Please try again later.",
            context.HttpContext.TraceIdentifier
        );
        await context.HttpContext.Response.WriteAsJsonAsync(errorResponse, token);
    };
});

//builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// ----------------------------
// JWT Authentication
// ----------------------------
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JwtSettings:Key is missing in configuration.");
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.FromMinutes(1)   // small drift tolerance only — NOT the token lifetime
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ActiveSubscription", policy =>
        policy.Requirements.Add(new SubscriptionRequirement()));
});

// Register the authorization handler
builder.Services.AddScoped<IAuthorizationHandler, SubscriptionAuthorizationHandler>();
// ----------------------------
// Application services
// ----------------------------
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddSharedServices();

// ----------------------------
// Hangfire — background job processing
// ----------------------------
builder.Services.AddHangfireServices(builder.Configuration);

// ----------------------------
// Background services
// ----------------------------
builder.Services.AddHostedService<TrialExpirationService>();

// ----------------------------
// Build the app
// ----------------------------
var app = builder.Build();

// ----------------------------
// Middleware pipeline
// ----------------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SocietyLedger API V1");
    c.RoutePrefix = string.Empty; 
});

app.UseHttpsRedirection();

// Secure headers
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    ctx.Response.Headers.Append("X-Frame-Options", "DENY");
    ctx.Response.Headers.Append("Referrer-Policy", "no-referrer");
    ctx.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    await next();
});

// Correlation ID middleware
app.UseMiddleware<SocietyLedger.Api.CorrelationIdMiddleware>();

app.UseCors("DefaultCorsPolicy");
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// ----------------------------
// Hangfire Dashboard
// Secured: accessible only to authenticated admin/super-admin users.
// Path: /hangfire
// ----------------------------
//app.UseHangfireDashboard("/hangfire", new DashboardOptions
//{
//    DashboardTitle = "SocietyLedger Jobs",
//    // Resolve the filter through DI so it can use IWebHostEnvironment / IConfiguration.
//    Authorization =
//    [
//        new HangfireAuthorizationFilter(
//            app.Services.GetRequiredService<IWebHostEnvironment>(),
//            app.Services.GetRequiredService<IConfiguration>())
//    ]
//});

//// ----------------------------
//// Recurring Jobs
//// 1st of every month at 00:05 server time (UTC) — Cron: "5 0 1 * *"
//// ----------------------------
//RecurringJob.AddOrUpdate<MonthlyBillingJob>(
//    recurringJobId: "monthly-billing",
//    methodCall:    job => job.ExecuteAsync(),
//    cronExpression: "5 0 1 * *",
//    new RecurringJobOptions
//    {
//        TimeZone = TimeZoneInfo.Utc
//    });

// Health check endpoint
app.MapHealthChecks("/health");

// ----------------------------
// API Version Set & Endpoints
// ----------------------------
var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(ApiConstants.API_VERSION_1_0))
    .ReportApiVersions()
    .Build();

app.MapGroup(ApiRoutes.AUTH)
   .MapAuthRoutes(RouteGroupNames.AUTHENTICATION, versionSet);

app.MapGroup(ApiRoutes.FLATS)
   .MapFlatRoutes(RouteGroupNames.FLAT, versionSet);

app.MapGroup(ApiRoutes.USERS)
   .MapUserRoutes(RouteGroupNames.USER, versionSet);

app.MapGroup(ApiRoutes.SUBSCRIPTIONS)
   .MapSubscriptionRoutes(RouteGroupNames.SUBSCRIPTION, versionSet);

app.MapGroup(ApiRoutes.INVOICES)
   .MapInvoiceRoutes(RouteGroupNames.INVOICE, versionSet);

app.MapGroup(ApiRoutes.PLANS)
   .MapPlanRoutes(RouteGroupNames.PLAN, versionSet);

app.MapGroup(ApiRoutes.PAYMENTS)
   .MapPaymentRoutes(RouteGroupNames.PAYMENT, versionSet);

app.MapGroup(ApiRoutes.MAINTENANCE_PAYMENTS)
   .MapMaintenancePaymentRoutes(RouteGroupNames.MAINTENANCE_PAYMENT, versionSet);

app.MapGroup(ApiRoutes.PAYMENT_MODES)
   .MapPaymentModeRoutes(RouteGroupNames.PAYMENT_MODE, versionSet);

app.MapGroup(ApiRoutes.EXPENSES)
   .MapExpenseRoutes(RouteGroupNames.EXPENSES, versionSet);

app.MapGroup(ApiRoutes.OPENING_BALANCE)
   .MapOpeningBalanceRoutes(RouteGroupNames.OPENING_BALANCE, versionSet);

app.MapGroup(ApiRoutes.BILLING)
   .MapBillingRoutes(RouteGroupNames.BILLING, versionSet);

app.MapGroup(ApiRoutes.SOCIETIES)
   .MapSocietyRoutes(RouteGroupNames.SOCIETY, versionSet);

app.MapGroup(ApiRoutes.NOTIFICATIONS)
   .MapNotificationRoutes(RouteGroupNames.NOTIFICATION, versionSet);

// Dashboard endpoints
app.MapDashboardEndpoints();

app.MapGroup(ApiRoutes.REPORTS)
   .MapReportRoutes(RouteGroupNames.REPORTS, versionSet);

app.Run();
