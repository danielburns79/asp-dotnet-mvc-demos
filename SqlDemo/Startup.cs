using System;
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
using SqlDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

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

            services.AddSingleton<IIdentityUserRepository>(new IdentityDbContext(new DbContextOptionsBuilder<IdentityDbContext>().UseSqlServer(Configuration["Data:CommandAPIConnection:ConnectionString"]).Options));
            services.AddSingleton<IClassRepository>(new ClassEntityFrameworkRepository(new DbContextOptionsBuilder<ClassEntityFrameworkRepository>().UseSqlServer(Configuration["Data:CommandAPIConnection:ConnectionString"]).Options));
            services.AddSingleton<IEnrolementRepository>(new EnrolementEntityFrameworkRepository(new DbContextOptionsBuilder<EnrolementEntityFrameworkRepository>().UseSqlServer(Configuration["Data:CommandAPIConnection:ConnectionString"]).Options));
            services.AddSingleton<IQuestionRepository>(new QuestionEntityFrameworkRepository(new DbContextOptionsBuilder<QuestionEntityFrameworkRepository>().UseSqlServer(Configuration["Data:CommandAPIConnection:ConnectionString"]).Options));
            services.AddSingleton<IProblemRepository>(new ProblemEntityFrameworkRepository(new DbContextOptionsBuilder<ProblemEntityFrameworkRepository>().UseSqlServer(Configuration["Data:CommandAPIConnection:ConnectionString"]).Options));
            services.AddSingleton<IAssignmentRepository>(new AssignmentEntityFrameworkRepository(new DbContextOptionsBuilder<AssignmentEntityFrameworkRepository>().UseSqlServer(Configuration["Data:CommandAPIConnection:ConnectionString"]).Options));
            
            services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(Configuration["Data:CommandAPIConnection:ConnectionString"]));
            //services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>().AddDefaultUI(UIFramework.Bootstrap4).AddEntityFrameworkStores<UserEntityFrameworkRepository>();
            services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>().AddEntityFrameworkStores<IdentityDbContext>().AddDefaultTokenProviders();

            //services.AddIdentity<IdentityUser, IdentityRole>(options => {
            //    options.Cookies.ApplicationCookie.LoginPath = "/Account/SignIn";
            //})
            //services.Configure<IdentityOptions>(options => {
            //    options.Cookies.ApplicationCookie.LoginPath = "/Account/SignIn";
            //});
            services.Configure<IdentityOptions>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;                
            });
            //services.Configure<IdentityOptions>(options => {
            //    options.SignIn.RequireConfirmedEmail = true;               
            //});

            services.AddTransient<IMessageService, FileMessageService>();

            services.AddMvc(o => 
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                o.Filters.Add(new AuthorizeFilter(policy));
            });

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
            //app.UseIdentity(); 
            app.UseAuthentication();
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
