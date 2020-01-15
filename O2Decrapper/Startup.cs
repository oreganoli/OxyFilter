using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace O2Decrapper
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvcWithDefaultRoute();
        }
    }
}