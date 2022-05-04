using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OAuth.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth.Services
{
    public static class ConfigureIdentity
    {
        public static void ConfigureIdentityM(this IServiceCollection service)
        {
            service.AddIdentity<IdentityUser, IdentityRole>(opt => 
            {
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequiredLength = 5;
                opt.Password.RequireDigit = false;
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<AppDbContext>();
        }
    }
}
