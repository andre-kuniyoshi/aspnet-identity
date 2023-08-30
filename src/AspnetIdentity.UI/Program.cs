using AspnetIdentity.Application.Extensions;
using AspnetIdentity.Infra.Data.Seed;
using AspnetIdentity.Infra.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AspnetIdentity.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddApplicationLayer();
            builder.Services.AddInfraLayer(builder.Configuration);

            var app = builder.Build();

            app.Lifetime.ApplicationStarted.Register(() => IdentityContextSeed.MigrateDatabase(app));

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "MinhaArea",
                pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            

            app.Run();
        }
    }
}