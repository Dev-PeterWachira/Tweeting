
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tweeting_book.Migrations;
using Tweeting_book.Services;
using Tweeting_book.Data;


namespace Tweeting_book.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));
                services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DataContext>();

                services.AddScoped<IPostService, PostService>();
            
        }
    }
}