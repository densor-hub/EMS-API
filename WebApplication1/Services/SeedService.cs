using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Services
{
    public class SeedService
    {

        public static async Task InitializeSimpleAsync(AppDbContext context)
        {
            var adminPositionId = new Guid("00000000-0000-0000-0000-000000000001");
            var systemUserId = new Guid("00000000-0000-0000-0000-000000000001");

            // 1. Create ADMIN position if missing
            var adminPosition = await context.Positions.FindAsync(adminPositionId);
            if (adminPosition == null)
            {
                adminPosition = Position.Create(
                    id: adminPositionId,
                    title: "ADMIN",
                    companyId: null,
                    status: true,
                    createdAt: DateTime.UtcNow,
                    createdBy: systemUserId,
                    description: "System Administrator"
                );
                context.Positions.Add(adminPosition);
                await context.SaveChangesAsync();
            }

            // 2. Get all routes and existing position routes for ADMIN
            var allRoutes = await context.ApplicationRoutes.ToListAsync();
            var existingPositionRouteIds = await context.PositionRoutes
                .Where(pr => pr.PositionId == adminPositionId)
                .Select(pr => pr.AppRouteId)
                .ToListAsync();

            // 3. Create missing PositionRoutes
            var routesToAdd = allRoutes.Where(r => !existingPositionRouteIds.Contains(r.Id));

            foreach (var route in routesToAdd)
            {
                var positionRoute = PositionRoutes.Create(
                    id: Guid.NewGuid(),
                    positionId: adminPositionId,
                    appRouteId: route.Id,
                    createdAt: DateTime.UtcNow,
                    createdBy: systemUserId
                );
                context.PositionRoutes.Add(positionRoute);
            }

            await context.SaveChangesAsync();
        }
        public static async Task SeedDefaultCompanyAndAdminAsync(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Check if default company already exists
            var defaultCompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var defaultCompany = await context.Companies
                .FirstOrDefaultAsync(c => c.Id == defaultCompanyId);

            if (defaultCompany == null)
            {
                defaultCompany = Company.Create(defaultCompanyId, "Default Company", "1234567890", "admin@company.com", true, DateTime.UtcNow);
                

                await context.Companies.AddAsync(defaultCompany);
                await context.SaveChangesAsync();
                Console.WriteLine("Default company created.");
            }


            // Create super admin user
            var adminEmail = "superadmin@company.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Super Administrator",
                    CompanyId = defaultCompanyId, // Required - assign to default company
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Status = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
                    await userManager.AddToRoleAsync(adminUser, "CompanyAdmin");
                    Console.WriteLine("Super admin user created.");
                }
                else
                {
                    Console.WriteLine("Failed to create super admin: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}