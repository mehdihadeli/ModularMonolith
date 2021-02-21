using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModularMonolith.Shared.Infrastructure.Api;
using ModularMonolith.Shared.Infrastructure.Exceptions;
using ModularMonolith.Shared.Infrastructure.Postgres;

#region Application Part
// https://andrewlock.net/when-asp-net-core-cant-find-your-controller-debugging-application-parts/
// https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts?view=aspnetcore-5.0
// ASP.NET CORE MVC ANATOMY (PART 1) – ADDMVCCORE
#endregion

[assembly: InternalsVisibleTo("ModularMonolith.Bootstrapper")]
namespace ModularMonolith.Shared.Infrastructure
{
    internal static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddControllers()
                // // will use for load different assembly with different controller and embedded razor views and add them dynamically as a application part
                // // we can use it in modular monolith to load some plugins
                // .AddApplicationPart()  
                .ConfigureApplicationPartManager(manager =>
                {
                    // enrich or framework with custom feature to able to work with internal controllers
                    manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
                });

            services.AddSingleton<ErrorHandlerMiddleware>();
            services.AddPostgres();

            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
            return app;
        }

        public static T GetOptions<T>(this IServiceCollection services, string sectionName) where T : new()
        {
            using var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var section = configuration.GetSection(sectionName);
            var options = new T();
            section.Bind(options);

            return options;
        }
    }
}