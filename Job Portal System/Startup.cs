using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Job_Portal_System.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Job_Portal_System
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<User>()
                .AddRoles<IdentityRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<SignalRHub>("/signalRHub");
            });

            //DatabaseSeeder.SeedData(env, context, userManager, roleManager);
            //DatabaseSeeder.SeedCompanies(context);
            //DatabaseSeeder.SeedSchools(context);
            //DatabaseSeeder.SeedSkills(context);
            //DatabaseSeeder.SeedJobSeekers(context, userManager, roleManager, 1000);
            {
                var count = context.Resumes.Count();
            }
            //DatabaseSeeder.ClearDatabase(context);

            //{
            //    var evaluations = context.Applicants
            //        .Where(a => a.JobVacancyId == "a0c7d5ee-1d02-4e21-8efa-f122e10baea9")
            //        .Select(a => FilesManager.Read<Evaluation>(env, "Evaluations", a.Id))
            //        .ToList();
            //}

            app.UseMvc();
        }
    }
}
