using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Monica.Core.DbModel.IdentityModel;
using Monica.Core.DbModel.Localization.Identity;

namespace Monica.Core.DbModel.Extension
{
    public static class DbContextExtensions
    {
        public static void AddDbContextCore(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>();
        }
    }
}
