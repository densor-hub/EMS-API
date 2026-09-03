using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Services
{
    public class SeedService
    {
        // Define menus with hierarchical structure
        // Level determines the order within the same parent
        private static readonly List<MenuDefinition> MenuDefinitions = new()
        {
            new MenuDefinition
            {
                Id = Guid.Parse("51c32656-ba18-4bb7-8594-e9deff2f4751"),
                Title = "Dashboard",
                Path = "/dashboard",
                Level = 1,  // Order: 1st in root
                Children = new List<MenuDefinition>()
            },
            new MenuDefinition
            {
                Id = Guid.Parse("5630bcf8-941b-49c4-a9ed-8655ceb7b659"),
                Title = "Transactions",
                Path = "/dashboard/transactions",
                Level = 2,  // Order: 2nd in root
                Children = new List<MenuDefinition>
                {
                    new MenuDefinition
                    {
                        Id = Guid.Parse("5b5ec551-d58b-4f4f-b695-b68374c0b428"),
                        Title = "Deposit",
                        Path = "/dashboard/transactions/deposit",
                        Level = 2.1  // Order: 1st in Transactions
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("bd5bc70e-e051-4d11-9edc-2692cd2276ff"),
                        Title = "Purchase",
                        Path = "/dashboard/transactions/purchase",
                        Level = 2.2  // Order: 2nd in Transactions
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("63f32299-2135-4704-b82e-516250845f09"),
                        Title = "Sale",
                        Path = "/dashboard/transactions/sale",
                        Level = 2.3  // Order: 3rd in Transactions
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("03305554-06a8-46ec-9227-751781e1619d"),
                        Title = "Stock",
                        Path = "/dashboard/transactions/stock",
                        Level = 2.4  // Order: 4th in Transactions
                    }
                }
            },
            new MenuDefinition
            {
                Id = Guid.Parse("5201b8ff-e381-4e8d-beba-2181862f996a"),
                Title = "Setup",
                Path = "/dashboard/setup",
                Level = 3,  // Order: 3rd in root
                Children = new List<MenuDefinition>
                {
                    new MenuDefinition
                    {
                        Id = Guid.Parse("0504516a-47b7-49b9-a083-88d2b6b4ad40"),
                        Title = "Roles",
                        Path = "/dashboard/setup/roles",
                        Level = 3.1  // Order: 1st in Setup
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("6201b8ff-e381-4e8d-beba-2181862f997a"),
                        Title = "Employees",
                        Path = "/dashboard/setup/employees",
                        Level = 3.2  // Order: 2nd in Setup
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("3bfe453d-2244-449b-aca2-e818c891dc7b"),
                        Title = "Suppliers",
                        Path = "/dashboard/setup/suppliers",
                        Level = 3.3  // Order: 3rd in Setup
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("a9925b1f-f0b8-49b6-881e-c977d829a3bd"),
                        Title = "Customers",
                        Path = "/dashboard/setup/customers",
                        Level = 3.4  // Order: 4th in Setup
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("bd6efe66-0e95-4857-8b84-e8c15ab60779"),
                        Title = "Shops",
                        Path = "/dashboard/setup/shops",
                        Level = 3.5  // Order: 5th in Setup
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("ca7c3a1a-5b18-4fb8-bdf5-0a2d4eb37629"),
                        Title = "Items",
                        Path = "/dashboard/setup/items",
                        Level =3.6  // Order: 6th in Setup
                    }
                }
            },
            new MenuDefinition
            {
                Id = Guid.Parse("265f0a27-ddff-4a46-9222-2424be683c22"),
                Title = "Reports",
                Path = "/dashboard/reports",
                Level = 4,  // Order: 4th in root
                Children = new List<MenuDefinition>()
            }
        };

        public static async Task SeedMenusHierarchicalAsync(AppDbContext context)
        {
            // Flatten the hierarchical structure and assign ParentIds
            var allMenus = new List<ApplicationRoute>();
            FlattenMenuDefinitions(MenuDefinitions, null, allMenus);

            // Get existing menus for change detection
            var existingMenus = await context.ApplicationRoutes.ToDictionaryAsync(r => r.Id);

            var menusToAdd = new List<ApplicationRoute>();
            var menusToUpdate = new List<ApplicationRoute>();

            foreach (var menu in allMenus)
            {
                if (existingMenus.TryGetValue(menu.Id, out var existingMenu))
                {
                    // Check for changes
                    bool hasChanges = false;

                    if (existingMenu.Title != menu.Title)
                    {
                        existingMenu.Title = menu.Title;
                        hasChanges = true;
                    }

                    if (existingMenu.Status != menu.Status)
                    {
                        existingMenu.Status = menu.Status;
                        hasChanges = true;
                    }

                    if (existingMenu.Level != menu.Level)
                    {
                        existingMenu.Level = menu.Level;
                        hasChanges = true;
                    }

                    if (existingMenu.ParentId != menu.ParentId)
                    {
                        existingMenu.ParentId = menu.ParentId;
                        hasChanges = true;
                    }

                    if (existingMenu.Path != menu.Path)
                    {
                        existingMenu.Path = menu.Path;
                        hasChanges = true;
                    }

                    if (hasChanges)
                    {
                        menusToUpdate.Add(existingMenu);
                    }
                }
                else
                {
                    menusToAdd.Add(menu);
                }
            }

            // Add new menus
            if (menusToAdd.Any())
            {
                await context.ApplicationRoutes.AddRangeAsync(menusToAdd);
            }

            // Update changed menus
            if (menusToUpdate.Any())
            {
                context.ApplicationRoutes.UpdateRange(menusToUpdate);
            }

            await context.SaveChangesAsync();
        }

        private static void FlattenMenuDefinitions(
            IEnumerable<MenuDefinition> menuDefinitions,
            Guid? parentId,
            List<ApplicationRoute> flattenedMenus,
            int depth = 0)
        {
            foreach (var definition in menuDefinitions.OrderBy(m => m.Level))
            {
                var menu = ApplicationRoute.Create(
                    definition.Id,
                    definition.Title,
                    true, // Status
                    definition.Level, // Level serves as OrderKey
                    parentId,
                    definition.Path
                );

                flattenedMenus.Add(menu);

                // Process children recursively
                if (definition.Children != null && definition.Children.Any())
                {
                    FlattenMenuDefinitions(definition.Children, definition.Id, flattenedMenus, depth + 1);
                }
            }
        }

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
            var allRoutes = await context.ApplicationRoutes
                .OrderBy(r => r.Level)  // Order by Level for consistent ordering
                .ToListAsync();

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

            await SeedService.SeedMenusHierarchicalAsync(context);

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
                    CompanyId = defaultCompanyId,
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

    // Helper class for menu definitions
    public class MenuDefinition
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public double Level { get; set; }  // Serves as OrderKey within the same parent
        public List<MenuDefinition> Children { get; set; } = new();
    }
}