using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DinkToPdf;
using DinkToPdf.Contracts;
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
            //DatabaseSeeder.ClearDatabase(context);
            //DatabaseSeeder.SeedJobSeekers(context, userManager, 1000);
            //var i = context.JobSeekers.Count();
            var ed = new[] {"Data Structures", "Business & Commercial Law", "Law" };
            var wo = new string[0];
            var sk = new string[0];
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // the code that you want to measure comes here
            var lengths = new[] {ed.Length, wo.Length, sk.Length};
            var maxCount = lengths.Max();
            var wob = wo.Length > 0;
            var edb = ed.Length > 0;
            var skb = sk.Length > 0;
            var resumes = context.Resumes
                .Include(r => r.WorkExperiences).ThenInclude(w => w.JobTitle)
                .Include(r => r.Educations).ThenInclude(e => e.FieldOfStudy)
                .Include(r => r.OwnedSkills).ThenInclude(s => s.Skill)
                .Where(r =>
                    r.WorkExperiences.Any(w => !wob || wo[0] == w.JobTitle.Title) &&
                    r.Educations.Any(w => !edb || ed[0] == w.FieldOfStudy.Title) &&
                    r.OwnedSkills.Any(w => !skb || sk[0] == w.Skill.Title))
                .ToList();

            for (var i = 1; i < maxCount; i++)
            {
                resumes = Filter(resumes, wo, ed, sk, i);
            }
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            app.UseMvc();
        }

        private static List<Resume> Filter(IEnumerable<Resume> resumes, IReadOnlyList<string> wo, 
            IReadOnlyList<string> ed, IReadOnlyList<string> sk, int i)
        {
            var wob = wo.Count > i;
            var edb = ed.Count > i;
            var skb = sk.Count > i;
            return resumes
                .Where(r =>
                    r.WorkExperiences.Any(w => !wob || wo[i] == w.JobTitle.Title) &&
                    r.Educations.Any(e => !edb || ed[i] == e.FieldOfStudy.Title) &&
                    r.OwnedSkills.Any(s => !skb || sk[i] == s.Skill.Title))
                .ToList();
        }
    }
}
