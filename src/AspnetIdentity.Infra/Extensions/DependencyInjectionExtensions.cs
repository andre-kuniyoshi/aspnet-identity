using AspnetIdentity.Infra.Data.Context;
using AspnetIdentity.Infra.Identity.Policies;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspnetIdentity.Infra.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddInfraLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("sqlServer")));


            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequireNonAlphanumeric = false;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "AspNetIdentity.Cookies";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.SlidingExpiration = true;
            });

            services.AddAuthorization(options =>
            {
                // Policies based in Roles
                options.AddPolicy("RequireUserAdminManagerRole",
                     policy => policy.RequireRole("User", "Admin", "Manager"));
            });

            services.AddAuthorization(options =>
            {
                // Policies based in Claims
                options.AddPolicy("IsAdminClaimAccess",
                     policy => policy.RequireClaim("RegisteredIn"));

                options.AddPolicy("IsAdminClaimAccess",
                     policy => policy.RequireClaim("IsAdmin", "true"));

                options.AddPolicy("IsEmployeeClaimAccess",
                     policy => policy.RequireClaim("IsEmployee", "true"));

                // Custom Policy
                options.AddPolicy("RegistationTimeMin", policy => {
                    policy.Requirements.Add(new RegistrationTimeRequirement(5));
                });
            });
            services.AddScoped<IAuthorizationHandler, RegistrationTimeHandler>();

            return services;
        }
    }
}
