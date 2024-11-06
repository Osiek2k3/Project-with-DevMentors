using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySpot.Application.Abstractions;
using MySpot.Core.Abstractions;
using MySpot.Infrastructure.DAL;
using MySpot.Infrastructure.Exceptions;
using MySpot.Infrastructure.Logging;
using MySpot.Infrastructure.Time;

[assembly: InternalsVisibleTo("My.Spot.Tests.Unit")]
namespace MySpot.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddControllers();
            var section = configuration.GetSection("app");
            services.Configure<AppOptions>(section);
            services.AddSingleton<ExceptionMiddleware>();

            services
                .AddPostgres(configuration)
                .AddSingleton<IClock, Clock>();

            var infrastructureAssembly = typeof(AppOptions).Assembly;
            services.Scan(s => s.FromAssemblies(infrastructureAssembly)
                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );

            services.AddCustomLogging();

            return services;
        }
        public static WebApplication UseInfrastructure(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.MapControllers();

            return app;
        }
    }
}
