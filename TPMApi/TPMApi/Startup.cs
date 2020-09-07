using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TPMApi.Controllers;
using TPMApi.Data;
using TPMApi.Middelware;
using TPMApi.Models;
using TPMApi.TokenProvider;

namespace TPMApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("TPMAConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTokenAuthentication(Configuration);

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<IdentityUser> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //--------------------------------------//
            /*
            var siginKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("this_is_a_secret_key"));

            var options = new TokenProvider.TokenOptions()
            {
                Audiance = "https://localhost:44339/",
                Issuer = "https://localhost:44339/",
                SigningCredentials = new SigningCredentials(siginKey, SecurityAlgorithms.HmacSha256),
                ValidateIssuerSigningKey = false,
                ValidateIssuer = true
            };

            app.UseMiddleware<TokenProviderMiddelware>(Options.Create(options));
            */
            //--------------------------------------//

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
