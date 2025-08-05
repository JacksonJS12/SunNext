using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using SunNext.Data;
using SunNext.Services.BlogPost;
using SunNext.Services.Data;
using SunNext.Services.Market;
using SunNext.Services.Simulation;
using SunNext.Services.SolarAsset;
using SunNext.Services.User;
using SunNext.Services.VirtualWallet;
using static SunNext.Common.GlobalConstants;

namespace SunNext.Web
{
    using System.Reflection;
    using SunNext.Data.Common;
    using SunNext.Data.Common.Repositories;
    using SunNext.Data.Models;
    using SunNext.Data.Repositories;
    using SunNext.Data.Seeding;
    using SunNext.Services.Mapping;
    using SunNext.Services.Messaging;
    using SunNext.Web.ViewModels;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using AutoMapper;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();
            
            await ConfigureAsync(app);

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var bulgarianCulture = new CultureInfo("bg-BG");

                options.DefaultRequestCulture = new RequestCulture(bulgarianCulture);
                options.SupportedCultures = new[] { bulgarianCulture };
                options.SupportedUICultures = new[] { bulgarianCulture };

                options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Add(new CustomRequestCultureProvider(async context =>
                {
                    return new ProviderCultureResult("bg-BG");
                }));
            });

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            }).AddRazorRuntimeCompilation();

            services.AddRazorPages();
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddSingleton(configuration);

            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            services.AddScoped<IBlogPostService, BlogPostService>();
            services.AddScoped<ISolarAssetService, SolarAssetService>();
            services.AddHttpClient<IMarketService, MarketService>();
            services.AddScoped<IMarketService, MarketService>();
            services.AddScoped<ISolarSimulatorService, SolarSimulationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IVirtualWalletService, VirtualWalletService>();


            services.AddAutoMapper(
                config => { },
                typeof(BlogProfile).Assembly,
                typeof(SolarAssetProfile).Assembly,
                typeof(MarketProfile).Assembly
            );

            services.AddTransient<IEmailSender, NullMessageSender>();
        }

        private static async Task ConfigureAsync(WebApplication app)
        {
            using (var serviceScope = app.Services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await dbContext.Database.MigrateAsync();
                await new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider);

                var simulatorService = serviceScope.ServiceProvider.GetRequiredService<ISolarSimulatorService>();
                await simulatorService.GenerateForAllAssetsAsync(TodayEESTTime);
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/error/500");

                app.UseStatusCodePages(async context =>
                {
                    var response = context.HttpContext.Response;
                    if (response.StatusCode == 404)
                    {
                        response.Redirect("/error/404");
                    }
                });

                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRequestLocalization();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapDefaultControllerRoute();
            app.MapRazorPages();
        }
    }
}