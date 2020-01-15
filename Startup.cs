using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace OxyFilter
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IInfoService, InfoService>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseMvc();
            app.UseStaticFiles();
        }
    }
}