using Discount.API.Repositories;
using Discount.API.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Discount.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(sgOptions =>
            {
                sgOptions.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Discount API",
                    Version = "v1"
                });
            });

            services.AddAutoMapper(typeof(DiscountRepository).Assembly);

            services.AddScoped<IDiscountRepository, DiscountRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // for global exception handling
            app.ConfigureExceptionHandler(env);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(sgr =>
            {
                sgr.SwaggerEndpoint("/swagger/v1/swagger.json", "Discount API V1");
            });
        }
    }
}
