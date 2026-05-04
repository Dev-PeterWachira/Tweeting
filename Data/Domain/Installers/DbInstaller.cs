
using Microsoft.EntityFrameworkCore;
using Tweeting_book.Migrations;
using Tweeting_book.Services;

namespace Tweeting_book.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));
                services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<DataContext>();

                services.AddScoped<IPostService, PostService>();
            
        }
    }
}