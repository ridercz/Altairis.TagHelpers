using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Altairis.TagHelpers.DemoApp {
    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            services.AddHttpContextAccessor();
            services.AddRazorPages();
            //services.Configure<TimeTagHelperOptions>(options => {
            //    options.GeneralDateFormatter = d => string.Format("{0:d}", d);
            //});
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapRazorPages();
            });
        }
    }
}
