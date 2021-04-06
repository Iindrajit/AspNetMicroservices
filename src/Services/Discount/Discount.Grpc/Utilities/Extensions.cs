using Discount.Grpc.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Discount.Grpc.Utilities
{
    public static class Extensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry=0)
        {
            int retryForAvailability = retry.Value;

            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    using (var connection = 
                        new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "DROP TABLE IF EXISTS Coupon";
                            command.ExecuteNonQuery();

                            command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY,
                                                                        ProductName VARCHAR(24) NOT NULL,
                                                                        Description TEXT,
                                                                        Amount INT)";
                            command.ExecuteNonQuery();

                            //Seed the discount details.
                            Seed.SeedCoupons(command);

                            logger.LogInformation("Migrated PostgreSQL database.");
                        }
                    }
                }
                catch(NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the PostgreSQL database.");

                    if(retryForAvailability < 20) //20 hard coded for now
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }
            }

            return host;
        }


        public static void ConfigureExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //Handle any exception centrally.
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                            await context.Response.WriteAsync("Sorry, something went wrong!");
                    });
                });
            }
        }
    }
}
