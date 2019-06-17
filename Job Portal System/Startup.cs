using System;
using System.Linq;
using DinkToPdf;
using DinkToPdf.Contracts;
using Job_Portal_System.BackgroundTasking;
using Job_Portal_System.BackgroundTasking.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Job_Portal_System.Data;
using Job_Portal_System.Dependencies;
using Job_Portal_System.Handlers;
using Job_Portal_System.Models;
using Job_Portal_System.SignalR;
using Microsoft.AspNetCore.SignalR;
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

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
                .AddDefaultTokenProviders()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddScoped<ITermsManager, TermsManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context, IHubContext<SignalRHub> hubContext,
            IServiceScopeFactory serviceScopeFactory, IBackgroundTaskQueue queue)
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
            //DatabaseSeeder.ClearDatabase(context);
            //DatabaseSeeder.SeedJobSeekers(context, userManager, 1000);
            //var count = context.JobSeekers.Count();
            //DatabaseSeeder.FixDatabase(context, env);
            //var jobVacancy = context.JobVacancies
            //    .Include(j => j.WorkExperienceQualifications).ThenInclude(q => q.JobTitle)
            //    .Include(j => j.EducationQualifications).ThenInclude(q => q.FieldOfStudy)
            //    .Include(j => j.DesiredSkills).ThenInclude(q => q.Skill)
            //    .Include(j => j.JobTypes)
            //    .Include(j => j.CompanyDepartment)
            //    .Include(j => j.User)
            //    .FirstOrDefault(j => j.Id == "faf10145-211b-4a99-ab16-6ca4bb356b10");
            var jobVacancyId = "faf10145-211b-4a99-ab16-6ca4bb356b10";
            queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context1 = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var hubContext1 = scopedServices.GetRequiredService<IHubContext<SignalRHub>>();

                    try
                    {
                        await AsyncHandler.Recommend(context1, hubContext1, token, jobVacancyId);
                    }
                    catch (Exception e)
                    {
                       
                    }
                }
            });
            app.UseMvc();
        }
    }
}
