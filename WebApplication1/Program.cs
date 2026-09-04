// ... other usings ...

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApplication1.DAL.Repository;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;
using WebApplication1.Services.TokenService;
using WebApplication1;
using WebApplication1.Services.PasswordService;
using WebApplication1.Services.Hubs.Repositories;
using ERDMS.Modules.Messaging.Core.DAL.Repositories;
using WebApplication1.Services.Emails.TemplateService;
using WebApplication1.Services.QrCodeService;
using WebApplication1.Services.Emails.EmailService.Entities;
//using WebApplication1.Services.Emails.EmailSenderService.Sender;
using WebApplication1.Services.Emails.EmailService;
using WebApplication1.Services;
using WebApplication1.Services.Emails.EmailService.Queuer;
using WebApplication1.Services.ControllerServices;
using WebApplication1.Services.TokenService.Handlers;
using WebApplication1.Repositories;
using WebApplication1.Domain.Repositories;
using WebApplication1.Domain.Interfaces;
using WebApplication1.Infrastructure.Repositories;
using WebApplication1.Services.ControllerServices.Implementations;
using WebApplication1.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration
    .GetSection("CorsSettings:AllowedOrigins")
    .Get<string[]>();

// 1. Configure Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// 3. Register services
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


//notifications
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();


//Handler of services
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

//Must be taken out
builder.Services.AddScoped<IPasswordGenerator, PasswordGenerator>();


//token generations
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPaymentTokenService, PaymentTokenService>();

//builder.Services.AddScoped<IPurchaseCancellationRepository, >();

builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddScoped<IEmailQueueRepository, EmailQueueRepository>();
builder.Services.AddScoped<IQrCodeService, QrCodeService>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddHostedService<EmailProcessor>();
builder.Services.AddHttpContextAccessor(); // ✅ Required for IHttpContextAccessor

// 4. Add Controllers
builder.Services.AddControllers();

// 5. Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(allowedOrigins ?? Array.Empty<string>())
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // using JWT authentication
    });
});

// 6. Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "EMS API", Version = "v1" });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

// Add SignalR
builder.Services.AddSignalR();

builder.Services.AddDistributedMemoryCache();

var app = builder.Build();


//seedData

// Configure pipeline
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

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseStaticFiles();

app.UseRouting();

// ⚠️ IMPORTANT: Your custom middleware should be HERE
// After UseRouting but before UseAuthentication/UseAuthorization
app.UseRefreshTokenMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


// Database migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        Console.WriteLine("Database migrated successfully.");

        await SeedService.InitializeSimpleAsync(context);

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();