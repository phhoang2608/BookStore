using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using BulkyBook.DataAccess.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BulkyBook.DataAccess.Respository;
using BulkyBook.DataAccess.Respository.IRepository;
using Microsoft.AspNetCore.Identity.UI.Services;
using BulkyBook.Utility;
using Stripe;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BulkyBook
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));//configuring SQL Server
            services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders() //when new user signed up, they need email to confirm.
                                                        //options => options.SignIn.RequireConfirmedAccount = true
                .AddEntityFrameworkStores<ApplicationDbContext>(); //default Identity with Identity Users adding EF using ApplicationDbContext 
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.Configure<EmailOptions>(Configuration); //match whatever email options we have in appsettings.json 
                                                             //and populate them in EmailOptions class in BulkyBook.Utilities
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            services.Configure<BrainTreeSettings>(Configuration.GetSection("BrainTree"));
            services.Configure<TwilioSettings>(Configuration.GetSection("Twilio"));
            services.AddSingleton<IBrainTreeGate, BrainTreeGate>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();//now we can inject it into any 
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages(); //To use Razor Pages in the application
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
            //configure facebook login  
            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = "3730544680353418";
                options.AppSecret = "c8807fdc9e14c0d43e197b9e37439177";
            });
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "1065169748234-os7480gcsprtq0tusin7l860khbmh19o.apps.googleusercontent.com";
                options.ClientSecret = "GmlC6sKkT73Rts8VzZZG0PgQ";
            });
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); //30 mins
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting(); //
            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];
            app.UseSession(); //enabled session using in ASP.NET core
            app.UseAuthentication(); //
            app.UseAuthorization(); //

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"); //routing for ASP.NET CORE for MVC
                endpoints.MapRazorPages(); //routing for ASP.NET CORE MVC for Razor Pages
            });
        }
    }
}
