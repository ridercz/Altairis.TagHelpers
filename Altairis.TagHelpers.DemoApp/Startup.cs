using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Altairis.TagHelpers.DemoApp {
    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc();
            //services.Configure<TimeTagHelperOptions>(options => {
            //    options.GeneralDateFormatter = d => string.Format("{0:d}", d);
            //});
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
        }
    }
}
