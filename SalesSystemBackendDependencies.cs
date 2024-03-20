using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesSystem.BLL;
using SalesSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem
{
    public static class SalesExtencion
    {
        public static void SalesSystemBackendDependencies(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<eToolsContext>(options);

            //AddTransient to be added

            services.AddTransient<CategoryServices>((serviceProvider) =>
                {
                    var context = serviceProvider.GetRequiredService<eToolsContext>();
                    //  Create an instance of the service and return the instance
                    return new CategoryServices(context);
                }
                );
            services.AddTransient<SaleServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<eToolsContext>();
                //  Create an instance of the service and return the instance
                return new SaleServices(context);
            }
                );
            services.AddTransient<StockItemServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<eToolsContext>();
                //  Create an instance of the service and return the instance
                return new StockItemServices(context);
            }
                );
        }
    }
}
