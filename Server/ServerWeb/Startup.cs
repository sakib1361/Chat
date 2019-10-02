using System;
using System.IO;
using ChatCore.Model.Core;
using ChatServer.Engine.Network;
using Jdenticon.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServerWeb.Engine.Database;

namespace ServerWeb
{
    public class Startup
    {
        readonly string file;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            file = Path.Combine(Environment.CurrentDirectory, "chatData.db");
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

            services.AddDbContext<LocalDBContext>(options =>
                    options.UseSqlite("Data Source=" + file));
            //services.AddDbContext<LocalDBContext>(options =>
            //       options.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\sakib\Documents\ChatServer.mdf;Integrated Security=True;Connect Timeout=30"));

            services.AddIdentity<IDUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                    .AddEntityFrameworkStores<LocalDBContext>()
                    .AddDefaultTokenProviders();
            //services.AddDefaultIdentity<IDUser>()
            //        .AddEntityFrameworkStores<LocalDBContext>();

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.SlidingExpiration = true;
                options.LoginPath = "/Accounts/Login";
            });


            services.AddScoped<MessageHandler>();
            services.AddScoped<ServerHandler>();
            services.AddScoped<APIHandler>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        private async void CreateUserRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IDUser>>();

            var adminRole = await roleManager.FindByNameAsync(ChatConstants.AdminRole);
            if (adminRole == null)
            {
                adminRole = new IdentityRole(ChatConstants.AdminRole);
                //create the roles and seed them to the database
                await roleManager.CreateAsync(adminRole);
                await roleManager.CreateAsync(new IdentityRole(ChatConstants.MemberRole));
            }
            //Assign Admin role to the main User here we have given our newly registered 
            //login id for Admin management
            var user = await userManager.FindByNameAsync("admin");
            if (user == null)
            {

                user = new IDUser()
                {
                    UserName = "admin",
                    Email = "sakib.buet51@outlook.com",
                };
                await userManager.CreateAsync(user, "pass_WORD_1234");
                await userManager.AddToRoleAsync(user, ChatConstants.AdminRole);
            }
          
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                //app.UseHsts();
            }

            CreateUserRoles(serviceProvider);
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });


            app.UseJdenticon();
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthorization();
            app.UseRouting();
            app.UseAuthentication();

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
