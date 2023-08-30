using AspnetIdentity.Application.Domain.Entities;
using AspnetIdentity.Infra.Data.Context;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspnetIdentity.Infra.Data.Seed
{
    public class IdentityContextSeed
    {
        public static async Task MigrateDatabase(WebApplication webApp, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            var scopedFactory = webApp.Services.GetService<IServiceScopeFactory>();

            using (var scope = scopedFactory.CreateScope())
            {
                var services = scope.ServiceProvider;

                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                var loggerContext = services.GetRequiredService<ILogger<AppDbContext>>();
                var loggerSeeder = services.GetRequiredService<ILogger<IdentityContextSeed>>();
                var context = services.GetService<AppDbContext>();
                var nameOrderContext = typeof(AppDbContext);

                try
                {
                    loggerContext.LogInformation("Starting migration database associated with context {DbContextName}", nameOrderContext);

                    context.Database.Migrate();

                    await SeedClientsAsync(context, loggerSeeder);
                    await SeedProductsAsync(context, loggerSeeder);

                    await SeedRolesAsync(roleManager);

                    await SeedUsersAsync(userManager);

                    await SeedUserClaimsAsync(userManager);

                    loggerContext.LogInformation("Migrated database associated with context {DbContextName} and seeded data", nameOrderContext);
                }
                catch (SqlException ex)
                {
                    loggerContext.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", nameOrderContext);

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;

                        Thread.Sleep(2000);
                        MigrateDatabase(webApp, retryForAvailability);
                    }
                }
            }
        }

        private static async Task SeedClientsAsync(AppDbContext context, ILogger<IdentityContextSeed> logger)
        {
            if (!context.Clients.Any())
            {
                context.Clients.AddRange(GetPreconfiguredClients());
                await context.SaveChangesAsync();
                logger.LogInformation("Seed Clients table");
            }
        }

        private static IEnumerable<Client> GetPreconfiguredClients()
        {
            return new List<Client>
            {
                new Client() { Name = "Andre", Email= "andre@teste.com", Age = 30 }
            };
        }

        private static async Task SeedProductsAsync(AppDbContext context, ILogger<IdentityContextSeed> logger)
        {
            if (!context.Products.Any())
            {
                context.Products.AddRange(GetPreconfiguredProducts());
                await context.SaveChangesAsync();
                logger.LogInformation("Seed Products table");
            }
        }

        private static IEnumerable<Product> GetPreconfiguredProducts()
        {
            return new List<Product>
            {
                new Product() { Name = "Book", Price= 10 },
                new Product() { Name = "Pen", Price= 3 }
            };
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("User"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "User";
                role.NormalizedName = "USER";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";
                role.NormalizedName = "ADMIN";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync("Manager"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Manager";
                role.NormalizedName = "MANAGER";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await roleManager.CreateAsync(role);
            }
        }

        private static async Task SeedUsersAsync(UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByEmailAsync("user@teste.com") == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "user@teste.com";
                user.Email = "user@teste.com";
                user.NormalizedUserName = "USER@TESTE.COM";
                user.NormalizedEmail = "USER@TESTE.COM";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult result = await userManager.CreateAsync(user, "Teste#2023");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }

            if (await userManager.FindByEmailAsync("admin@teste.com") == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "admin@teste.com";
                user.Email = "admin@teste.com";
                user.NormalizedUserName = "ADMIN@TESTE.COM";
                user.NormalizedEmail = "ADMIN@TESTE.COM";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult result = await userManager.CreateAsync(user, "Teste#2023");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            if (await userManager.FindByEmailAsync("manager@teste.com") == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "manager@teste.com";
                user.Email = "manager@teste.com";
                user.NormalizedUserName = "MANAGER@TESTE.COM";
                user.NormalizedEmail = "MANAGER@TESTE.COM";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult result = await userManager.CreateAsync(user, "Teste#2023");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Manager");
                }
            }
        }

        private static async Task SeedUserClaimsAsync(UserManager<IdentityUser> userManager)
        {
            try
            {
                //user 1
                IdentityUser user1 = await userManager.FindByEmailAsync("manager@teste.com");
                if (user1 is not null)
                {
                    var claimList = (await userManager.GetClaimsAsync(user1))
                                                       .Select(p => p.Type);

                    if (!claimList.Contains("RegisteredIn"))
                    {
                        var claimResult1 = await userManager.AddClaimAsync(user1,
                                 new Claim("RegisteredIn", "03/03/2021"));
                    }
                }

                // user 2
                IdentityUser user2 = await userManager.FindByEmailAsync("user@teste.com");
                if (user2 is not null)
                {
                    var claimList = (await userManager.GetClaimsAsync(user2))
                                                       .Select(p => p.Type);

                    if (!claimList.Contains("RegisteredIn"))
                    {
                        var claimResult1 = await userManager.AddClaimAsync(user2,
                                 new Claim("RegisteredIn", "01/01/2020"));
                    }
                }
                //user 3
                IdentityUser user3 = await userManager.FindByEmailAsync("admin@teste.com");
                if (user3 is not null)
                {
                    var claimList = (await userManager.GetClaimsAsync(user3))
                                                       .Select(p => p.Type);

                    if (!claimList.Contains("RegisteredIn"))
                    {
                        var claimResult1 = await userManager.AddClaimAsync(user3,
                                 new Claim("RegisteredIn", "02/02/2017"));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
