using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shamia.DataAccessLayer
{
    public static class Injection
    {
        public static IServiceCollection AddProjectServices(
                      this IServiceCollection services,
                      IConfiguration config)
        {
            services.AddDbContext<ShamiaDbContext>
                (
                    options => options.UseSqlServer(config.GetConnectionString("TestDb"))
                );

            return services;
        }
    }
}
