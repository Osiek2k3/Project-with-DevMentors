using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MySpot.Application.Abstractions;
using MySpot.Core.Abstractions;
using MySpot.Infrastructure.Auth;
using MySpot.Infrastructure.DAL;
using MySpot.Infrastructure.Exceptions;
using MySpot.Infrastructure.Logging;
using MySpot.Infrastructure.Security;
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
            services.AddSecurity();
            services.AddAuth(configuration);
            services.AddHttpContextAccessor();

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
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(swagger =>
            {
                swagger.EnableAnnotations();
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MySpot API",
                    Version = "v1"
                });
            });

            return services;
        }
        public static WebApplication UseInfrastructure(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseSwagger();
            app.UseReDoc(reDoc =>
            {
                reDoc.RoutePrefix = "docs";
                reDoc.DocumentTitle = "MySpot API";
                reDoc.SpecUrl("/swagger/v1/swagger.json");
            });
            app.UseAuthentication();
            app.UseAuthorization(); 
            app.MapControllers();

            return app;
        }
    }
}
