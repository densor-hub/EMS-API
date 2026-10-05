using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Services
{
    public class SeedService
    {
        // Current menu structure — mirrors the navItems array.
        // Level is a global ordering key (lower = earlier).
        private static readonly List<MenuDefinition> MenuDefinitions = new()
        {
            new MenuDefinition
            {
                Id = Guid.Parse("8b1e4f7a-2c3d-4e5f-9a6b-7c8d9e0f1a2b"),
                Title = "Dashboard",
                Path = "/dashboard",
                Level = 1,
            },
            new MenuDefinition
            {
                Id = Guid.Parse("a1f2e3d4-b5c6-4a78-9e0f-1a2b3c4d5e6f"),
                Title = "Sales",
                Path = "/sales",
                Level = 2,
                Children = new List<MenuDefinition>
                {
                    new MenuDefinition
                    {
                        Id = Guid.Parse("a2f3e4d5-c6b7-4a89-9f1a-2b3c4d5e6f7a"),
                        Title = "Sale",
                        Path = "/sales/sale",
                        Level = 2.1,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("a3f4e5d6-d7c8-4a9a-9a2b-3c4d5e6f7a8b"),
                        Title = "Deliveries",
                        Path = "/sales/deliveries",
                        Level = 2.2,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("a4f5e6d7-e8d9-4aab-9b3c-4d5e6f7a8b9c"),
                        Title = "Customer Sales",
                        Path = "/sales/customer-sales",
                        Level = 2.3,
                    },
                },
            },
            new MenuDefinition
            {
                Id = Guid.Parse("b1a2b3c4-d5e6-4f78-9a0b-1c2d3e4f5a6b"),
                Title = "Disbursements",
                Path = "/disbursements",
                Level = 3,
                Children = new List<MenuDefinition>
                {
                    new MenuDefinition
                    {
                        Id = Guid.Parse("b2a3b4c5-e6f7-4a89-9b1c-2d3e4f5a6b7c"),
                        Title = "Disbursements",
                        Path = "/disbursements/disbursement",
                        Level = 3.1,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("b3a4b5c6-f7a8-4b9a-9c2d-3e4f5a6b7c8d"),
                        Title = "Approval",
                        Path = "/disbursements/approval",
                        Level = 3.2,
                    },
                },
            },
            new MenuDefinition
            {
                Id = Guid.Parse("5e6f7a8b-9c0d-4e1f-9a2b-3c4d5e6f7a8b"), // keep original Stock Management Id
                Title = "Stock Management",
                Path = "/stock-management/stock",
                Level = 4,
                Children = new List<MenuDefinition>
                {
                    new MenuDefinition
                    {
                        Id = Guid.Parse("0d1e2f3a-4b5c-4d6e-9f7a-8b9c0d1e2f3a"),
                        Title = "Stock Lock",
                        Path = "/stock-management/stock-lock",
                        Level = 4.1,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("a1b2c3d4-e5f6-4a7b-9c8d-0e1f2a3b4c5d"),
                        Title = "Stock Take",
                        Path = "/stock-management/stock-take",
                        Level = 4.2,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("f7a8b9c0-d1e2-4f3a-9b4c-5d6e7f8a9b0c"),
                        Title = "Stock Verification",
                        Path = "/stock-management/stock-verification",
                        Level = 4.3,
                    },
                },
            },
            new MenuDefinition
            {
                Id = Guid.Parse("c1d2e3f4-a5b6-4c78-9d0e-1f2a3b4c5d6e"),
                Title = "Transfers",
                Path = "/transfers",
                Level = 5,
                Children = new List<MenuDefinition>
                {
                    new MenuDefinition
                    {
                        Id = Guid.Parse("c2d3e4f5-b6c7-4d89-9e1f-2a3b4c5d6e7f"),
                        Title = "Requests",
                        Path = "/transfers/requests",
                        Level = 5.1,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("c3d4e5f6-c7d8-4e9a-9f2a-3b4c5d6e7f8a"),
                        Title = "Manager Check",
                        Path = "/transfers/manager-check",
                        Level = 5.2,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("c4d5e6f7-d8e9-4fab-9a3b-4c5d6e7f8a9b"),
                        Title = "Deliveries",
                        Path = "/transfers/deliveries",
                        Level = 5.3,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("c5d6e7f8-e9fa-4abc-9b4c-5d6e7f8a9b0c"),
                        Title = "Receivals",
                        Path = "/transfers/receivals",
                        Level = 5.4,
                    },
                },
            },
            new MenuDefinition
            {
                Id = Guid.Parse("d1e2f3a4-b5c6-4d78-9e0f-1a2b3c4d5e6f"),
                Title = "Purchases",
                Path = "/purchases",
                Level = 6,
                Children = new List<MenuDefinition>
                {
                    new MenuDefinition
                    {
                        Id = Guid.Parse("d2e3f4a5-c6b7-4d89-9f1a-2b3c4d5e6f7a"),
                        Title = "Request",
                        Path = "/purchases/request",
                        Level = 6.1,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("d3e4f5a6-d7c8-4d9a-9a2b-3c4d5e6f7a8b"),
                        Title = "Manager Check",
                        Path = "/purchases/manager-check",
                        Level = 6.2,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("d4e5f6a7-e8d9-4dab-9b3c-4d5e6f7a8b9c"),
                        Title = "Receivals",
                        Path = "/purchases/receivals",
                        Level = 6.3,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("d5e6f7a8-f9ea-4dbc-9c4d-5e6f7a8b9c0d"),
                        Title = "Payments",
                        Path = "/purchases/payments",
                        Level = 6.4,
                    },
                },
            },
            new MenuDefinition
            {
                Id = Guid.Parse("2f3a4b5c-6d7e-4f8a-9b0c-1d2e3f4a5b6c"),
                Title = "Set up",
                Path = "/dashboard/setup",
                Level = 7,
                Children = new List<MenuDefinition>
                {
                    new MenuDefinition
                    {
                        Id = Guid.Parse("7a8b9c0d-1e2f-4a3b-9c4d-5e6f7a8b9c0d"),
                        Title = "Shops",
                        Path = "/setup/shops",
                        Level = 7.1,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("c1d2e3f4-a5b6-4c7d-9e8f-0a1b2c3d4e5f"),
                        Title = "Roles & Permissions",
                        Path = "/setup/roles",
                        Level = 7.2,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("8b9c0d1e-2f3a-4b4c-9d5e-6f7a8b9c0d1e"),
                        Title = "Employees",
                        Path = "/setup/employees",
                        Level = 7.3,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("d4e5f6a7-b8c9-4d0e-9f1a-2b3c4d5e6f7a"),
                        Title = "Items",
                        Path = "/setup/items",
                        Level = 7.4,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("f1e2d3c4-b5a6-4978-8e9f-0a1b2c3d4e5f"),
                        Title = "Customers",
                        Path = "/setup/customers",
                        Level = 7.5,
                    },
                    new MenuDefinition
                    {
                        Id = Guid.Parse("e5f6a7b8-c9d0-4e1f-9a2b-3c4d5e6f7a8b"),
                        Title = "Suppliers",
                        Path = "/setup/suppliers",
                        Level = 7.6,
                    },
                },
            },
            new MenuDefinition
            {
                Id = Guid.Parse("3a4b5c6d-7e8f-4a9b-9c0d-1e2f3a4b5c6d"),
                Title = "Reports",
                Path = "/reports",
                Level = 8,
            },
        };

        // Menus from previous versions that are no longer in the current nav.
        // These will be deleted during seeding.
        private static readonly List<Guid> RelegatedMenuIds = new()
        {
            // POS group (replaced by Sales)
            Guid.Parse("3f9c2a7e-6b4d-4c8a-9e1f-5a2b3c4d5e6f"), // POS
            Guid.Parse("c4d5e6f7-8a9b-4c1d-9e2f-3a4b5c6d7e8f"), // POS > Sale
            Guid.Parse("1a2b3c4d-5e6f-4a7b-8c9d-0e1f2a3b4c5d"), // POS > Receipt

            // Operations group (split into Purchases / Disbursements / Customer Sales)
            Guid.Parse("7e8f9a0b-1c2d-4e3f-9a4b-5c6d7e8f9a0b"), // Operations
            Guid.Parse("2b3c4d5e-6f7a-4b8c-9d0e-1f2a3b4c5d6e"), // Operations > Purchases
            Guid.Parse("9c0d1e2f-3a4b-4c5d-9e6f-7a8b9c0d1e2f"), // Operations > Customer Sales
            Guid.Parse("4d5e6f7a-8b9c-4d0e-9f1a-2b3c4d5e6f7a"), // Operations > Disbursements

            // Transfers submenus that moved out of Stock Management
            Guid.Parse("6e7f8a9b-0c1d-4e2f-9a3b-4c5d6e7f8a9b"), // Stock Management > Stock Transfer
            Guid.Parse("b8c9d0e1-f2a3-4b4c-9d5e-6f7a8b9c0d1e"), // Stock Management > Stock Transfer Approval
        };

        public static async Task SeedMenusHierarchicalAsync(AppDbContext context)
        {
            var allMenus = new List<ApplicationRoute>();
            FlattenMenuDefinitions(MenuDefinitions, null, allMenus);

            var existingMenus = await context.ApplicationRoutes
                .AsNoTracking()
                .ToDictionaryAsync(r => r.Id);

            var menusToAdd = new List<ApplicationRoute>();
            var menusToUpdate = new List<ApplicationRoute>();

            foreach (var menu in allMenus)
            {
                if (existingMenus.TryGetValue(menu.Id, out var existingMenu))
                {
                    bool hasChanges =
                        existingMenu.Title != menu.Title ||
                        existingMenu.Status != menu.Status ||
                        existingMenu.Level != menu.Level ||
                        existingMenu.ParentId != menu.ParentId ||
                        existingMenu.Path != menu.Path;

                    if (hasChanges)
                    {
                        context.ApplicationRoutes.Update(menu);
                        menusToUpdate.Add(menu);
                    }
                }
                else
                {
                    menusToAdd.Add(menu);
                }
            }

            if (menusToAdd.Count > 0)
                await context.ApplicationRoutes.AddRangeAsync(menusToAdd);

            if (menusToUpdate.Count > 0)
                context.ApplicationRoutes.UpdateRange(menusToUpdate);

            await context.SaveChangesAsync();

            // ✅ Delete relegated menus (cascades/removes any PositionRoutes tied to them)
            await DeleteRelegatedMenusAsync(context);

            context.ChangeTracker.Clear();
        }

        private static async Task DeleteRelegatedMenusAsync(AppDbContext context)
        {
            if (RelegatedMenuIds.Count == 0) return;

            // Delete dependent PositionRoutes first (avoid FK violation)
            var positionRoutes = await context.PositionRoutes
                .Where(pr => RelegatedMenuIds.Contains(pr.AppRouteId))
                .ToListAsync();

            if (positionRoutes.Count > 0)
            {
                context.PositionRoutes.RemoveRange(positionRoutes);
                await context.SaveChangesAsync();
            }

            // Delete the menus themselves
            var menusToDelete = await context.ApplicationRoutes
                .Where(r => RelegatedMenuIds.Contains(r.Id))
                .ToListAsync();

            if (menusToDelete.Count > 0)
            {
                context.ApplicationRoutes.RemoveRange(menusToDelete);
                await context.SaveChangesAsync();
            }
        }

        private static void FlattenMenuDefinitions(
            IEnumerable<MenuDefinition> menuDefinitions,
            Guid? parentId,
            List<ApplicationRoute> flattenedMenus)
        {
            foreach (var definition in menuDefinitions.OrderBy(m => m.Level))
            {
                var menu = ApplicationRoute.Create(
                    definition.Id,
                    definition.Title,
                    true,
                    definition.Level,
                    parentId,
                    definition.Path
                );

                flattenedMenus.Add(menu);

                if (definition.Children != null && definition.Children.Any())
                {
                    FlattenMenuDefinitions(definition.Children, definition.Id, flattenedMenus);
                }
            }
        }

        public static async Task InitializeSimpleAsync(AppDbContext context)
        {
            var adminPositionId = new Guid("00000000-0000-0000-0000-000000000001");
            var systemUserId = new Guid("00000000-0000-0000-0000-000000000001");

            // 1. Ensure ADMIN position exists
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

            // 2. Load all routes (no tracking)
            var allRoutes = await context.ApplicationRoutes
                .AsNoTracking()
                .OrderBy(r => r.Level)
                .ToListAsync();

            var existingPositionRouteIds = await context.PositionRoutes
                .AsNoTracking()
                .Where(pr => pr.PositionId == adminPositionId)
                .Select(pr => pr.AppRouteId)
                .ToListAsync();

            // 3. Create missing PositionRoutes
            var routesToAdd = allRoutes
                .Where(r => !existingPositionRouteIds.Contains(r.Id))
                .ToList();

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
            context.ChangeTracker.Clear();
        }

        public static async Task SeedDefaultCompanyAndAdminAsync(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // 0. Ensure roles exist
            foreach (var roleName in new[] { "SuperAdmin", "CompanyAdmin" })
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    Console.WriteLine($"Role '{roleName}' created.");
                }
            }

            // 1. Default company
            var defaultCompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var defaultCompany = await context.Companies
                .FirstOrDefaultAsync(c => c.Id == defaultCompanyId);

            if (defaultCompany == null)
            {
                defaultCompany = Company.Create(
                    defaultCompanyId, "Default Company", "1234567890",
                    "admin@company.com", true, DateTime.UtcNow);
                await context.Companies.AddAsync(defaultCompany);
                await context.SaveChangesAsync();
                Console.WriteLine("Default company created.");
            }

            // 2. Super admin user
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

                if (!result.Succeeded)
                {
                    Console.WriteLine(
                        "Failed to create super admin: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    return;
                }
                Console.WriteLine("Super admin user created.");
            }

            // 3. Ensure super admin has both roles (idempotent)
            var currentRoles = await userManager.GetRolesAsync(adminUser);
            foreach (var roleName in new[] { "SuperAdmin", "CompanyAdmin" })
            {
                if (!currentRoles.Contains(roleName))
                {
                    var roleResult = await userManager.AddToRoleAsync(adminUser, roleName);
                    if (roleResult.Succeeded)
                        Console.WriteLine($"Assigned role '{roleName}' to super admin.");
                    else
                        Console.WriteLine(
                            $"Failed to assign role '{roleName}': " +
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    // Helper class for menu definitions
    public class MenuDefinition
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public double Level { get; set; }
        public List<MenuDefinition> Children { get; set; } = new();
    }
}