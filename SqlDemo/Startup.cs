﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlDemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;

namespace SqlDemo
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

            // UserSqlRepository does direct SQL queries, UserContext uses entity framework
            //services.AddSingleton<IUserRepository>(new UserSqlRepository(Configuration["Data:CommandAPIConnection:ConnectionString"]));
            // the follow is how to initialize UsersController with a DbContext, but we're going to use IUserRepository instead
            //services.AddDbContext<UserContext>(opt => opt.UseSqlServer(Configuration["Data:CommandAPIConnection:ConnectionString"]));
            services.AddSingleton<IUserRepository>(new UserEntityFrameworkRepository(new DbContextOptionsBuilder<UserEntityFrameworkRepository>().UseSqlServer(Configuration["Data:CommandAPIConnection:ConnectionString"]).Options));
            services.AddSingleton<IClassRepository>(new ClassEntityFrameworkRepository(new DbContextOptionsBuilder<ClassEntityFrameworkRepository>().UseSqlServer(Configuration["Data:CommandAPIConnection:ConnectionString"]).Options));
            services.AddSingleton<IEnrolementRepository>(new EnrolementEntityFrameworkRepository(new DbContextOptionsBuilder<EnrolementEntityFrameworkRepository>().UseSqlServer(Configuration["Data:CommandAPIConnection:ConnectionString"]).Options));
            services.AddSingleton<IQuestionRepository>(new QuestionEntityFrameworkRepository(new DbContextOptionsBuilder<QuestionEntityFrameworkRepository>().UseSqlServer(Configuration["Data:CommandAPIConnection:ConnectionString"]).Options));
            services.AddSingleton<IProblemRepository>(new ProblemEntityFrameworkRepository(new DbContextOptionsBuilder<ProblemEntityFrameworkRepository>().UseSqlServer(Configuration["Data:CommandAPIConnection:ConnectionString"]).Options));
            services.AddSingleton<IAssignmentRepository>(new AssignmentEntityFrameworkRepository(new DbContextOptionsBuilder<AssignmentEntityFrameworkRepository>().UseSqlServer(Configuration["Data:CommandAPIConnection:ConnectionString"]).Options));
            
            services.AddDbContext<UserEntityFrameworkRepository>(options => options.UseSqlServer(Configuration["Data:CommandAPIConnection:ConnectionString"]));
            services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>().AddDefaultUI(UIFramework.Bootstrap4).AddEntityFrameworkStores<UserEntityFrameworkRepository>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

#if false
                routes.MapRoute(
                    name: "childapi",
                    template: "api/Parent/{id}/Child/{param}",
                    defaults: new { controller = "Child"/*, param = RouteParameter.Optional*/ }
                );

                routes.MapRoute(
                        name: "DefaultApi",
                        routeTemplate: "api/{controller}/{id}",
                        defaults: new { id = RouteParameter.Optional }
                );
#endif
            });
        }
    }
}
