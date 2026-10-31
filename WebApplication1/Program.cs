// Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApplication1.DAL;
using WebApplication1.DAL.Repository;
using WebApplication1.DAL.Repositories;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;
using WebApplication1.Infrastructure.Repositories;
using WebApplication1.Repositories;
using WebApplication1.Services;
using WebApplication1.Services.ControllerServices;
using WebApplication1.Services.ControllerServices.Implementations;
using WebApplication1.Services.Emails.EmailService;
using WebApplication1.Services.Emails.EmailService.Entities;
using WebApplication1.Services.Emails.EmailService.Queuer;
using WebApplication1.Services.Emails.TemplateService;
using WebApplication1.Services.Hubs.Repositories;
using WebApplication1.Services.PasswordService;
using WebApplication1.Services.QrCodeService;
using WebApplication1.Services.TokenService;
using WebApplication1.Services.TokenService.Handlers;
using ERDMS.Modules.Messaging.Core.DAL.Repositories;
using Resend;
using WebApplication1;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────
// 1. Database
// ─────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─────────────────────────────────────────────────────────────
// 2. Identity
// ─────────────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail           = true;
    options.Password.RequireDigit             = true;
    options.Password.RequiredLength           = 8;
    options.Password.RequireNonAlphanumeric   = true;
    options.Password.RequireUppercase         = true;
    options.Password.RequireLowercase         = true;
    options.Lockout.DefaultLockoutTimeSpan    = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts   = 5;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
    o.TokenLifespan = TimeSpan.FromHours(24));

// ─────────────────────────────────────────────────────────────
// 3. JWT options — bind ONCE, validate ONCE
//    (was incorrectly bound to "EmailSettings" further down)
// ─────────────────────────────────────────────────────────────
builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Secret) && o.Secret.Length >= 32,
              "Jwt:Secret must be at least 32 characters")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer),   "Jwt:Issuer is required")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Audience), "Jwt:Audience is required")
    .Validate(o => o.RefreshTokenExpirationDays > 0,
              "Jwt:RefreshTokenExpirationDays must be > 0")
    .ValidateOnStart();

// ─────────────────────────────────────────────────────────────
// 4. JWT bearer authentication (MISSING in the original)
//    Use the same JwtOptions instance the TokenService uses.
// ─────────────────────────────────────────────────────────────
var jwtOptionsSnapshot = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException("Jwt configuration section missing.");

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptionsSnapshot.Secret));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken            = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = signingKey,
            ValidateIssuer           = true,
            ValidIssuer              = jwtOptionsSnapshot.Issuer,
            ValidateAudience         = true,
            ValidAudience            = jwtOptionsSnapshot.Audience,
            ValidateLifetime         = true,
            ClockSkew                = TimeSpan.FromSeconds(jwtOptionsSnapshot.ClockSkewSeconds)
        };
    });

builder.Services.AddAuthorization();

// ─────────────────────────────────────────────────────────────
// 5. Domain repositories
// ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IDisbursementExternalResponseRepository, DisbursementExternalResponseRepository>();
builder.Services.AddScoped<IEmployeeLocationRepository, EmployeeLocationRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IFinancialDepositRepository, FinancialDepositRepository>();
builder.Services.AddScoped<IFinancialServiceProviderContactPersonRepository, FinancialServiceProviderContactPersonRepository>();
builder.Services.AddScoped<IFinancialServiceProviderRepository, FinancialServiceProviderRepository>();
builder.Services.AddScoped<IItemLocationRepository, ItemLocationRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IStockLockDownRequestRepository, StockLockDownRequestRepository>();
builder.Services.AddScoped<IStockLockDownItemRepository, StockLockDownItemRepository>();
builder.Services.AddScoped<IStockTakeItemSubmissionRepository, StockTakeItemSubmissionRepository>();
builder.Services.AddScoped<ISupplierLocationRepository, SupplierLocationRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ITransactionCodeRepository, TransactionCodeRepository>();
builder.Services.AddScoped<ITransactionItemsDeliveredRepository, TransactionItemsDeliveredRepository>();
builder.Services.AddScoped<ITransactionPaymentRepository, TransactionPaymentRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVehicleAssignmentRepository, VehicleAssignmentRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IStockLevelRepository, StockLevelRepository>();
builder.Services.AddScoped<IItemSplitSetupRepository, ItemSplitSetupRepository>();
builder.Services.AddScoped<IItemSplitTransactionRepository, ItemSplitTransactionRepository>();
builder.Services.AddScoped<IStockAdjustmentRepository, StockAdjustmentRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// ─────────────────────────────────────────────────────────────
// 6. Application services
// ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IFinancialServiceProviderService, FinancialServiceProviderService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IStockTransferService, StockTransferService>();
builder.Services.AddScoped<IStockLockDownService, StockLockDownService>();
builder.Services.AddScoped<IVehicleAssignmentService, VehicleAssignmentService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IItemSplitService, ItemSplitService>();
builder.Services.AddScoped<IStockAdjustmentService, StockAdjustmentService>();

builder.Services.AddScoped<IPasswordGenerator, PasswordGenerator>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPaymentTokenService, PaymentTokenService>();
builder.Services.AddScoped<TokenCookieHelper>();

builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IEmailQueueRepository, EmailQueueRepository>();
builder.Services.AddScoped<IQrCodeService, QrCodeService>();

// ─────────────────────────────────────────────────────────────
// 7. Email settings (was wrongly binding JwtOptions to this)
// ─────────────────────────────────────────────────────────────
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

var emailService = builder.Configuration
    .GetValue<string>("EmailSettings:EmailService")?
    .Trim()
    .ToLowerInvariant();

switch (emailService)
{
    case "resend":
        builder.Services.AddHttpClient<ResendClient>();
        builder.Services.Configure<ResendClientOptions>(o =>
            o.ApiToken = builder.Configuration["EmailSettings:ResendApiToken"]!);
        builder.Services.AddTransient<IResend, ResendClient>();
        builder.Services.AddScoped<IEmailSenderService, ResendEmailSenderService>();
        break;

    case "brevo":
        builder.Services.AddHttpClient<IEmailSenderService, BrevoEmailSenderService>(c =>
            c.Timeout = TimeSpan.FromSeconds(30));
        break;

    case "smtp":
        builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
        break;

    default:
        throw new InvalidOperationException(
            $"EmailSettings:EmailService must be 'resend', 'brevo', or 'smtp'. " +
            $"Got: '{emailService ?? "<null>"}'.");
}

builder.Services.AddHostedService<EmailProcessor>();
builder.Services.AddHttpContextAccessor();

// ─────────────────────────────────────────────────────────────
// 8. Controllers
// ─────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ─────────────────────────────────────────────────────────────
// 9. CORS — must match JwtOptions.RefreshHeaderName and refresh cookie name.
//    WithCredentials is required to send/receive the refresh cookie.
// ─────────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("CorsSettings:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders(jwtOptionsSnapshot.RefreshHeaderName);
    });
});

// ─────────────────────────────────────────────────────────────
// 10. Swagger
// ─────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "EMS API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name        = "Authorization",
        In          = ParameterLocation.Header,
        Type        = SecuritySchemeType.ApiKey,
        Scheme      = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddSignalR();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// ─────────────────────────────────────────────────────────────
// 11. Pipeline — order matters
// ─────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// CORS must run BEFORE auth so preflight OPTIONS gets CORS headers.
app.UseCors("AllowSpecificOrigins");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// If this middleware looks at HttpContext.User it must run AFTER UseAuthentication.
app.UseRefreshTokenMiddleware();

app.MapControllers();

// ─────────────────────────────────────────────────────────────
// 12. Migrate + seed
// ─────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

        await SeedService.SeedMenusHierarchicalAsync(context);
        await SeedService.InitializeSimpleAsync(context);

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        // await SeedService.SeedDefaultCompanyAndAdminAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();