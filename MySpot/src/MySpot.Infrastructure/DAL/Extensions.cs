﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySpot.Application.Abstractions;

using MySpot.Core.Repositories;
using MySpot.Infrastructure.DAL.Decorators;
using MySpot.Infrastructure.DAL.Repositories;
using MySpot.Infrastructure.Logging.Decorators;

namespace MySpot.Infrastructure.DAL
{
    internal static class Extensions
    {
        private const string SectionName = "postgres";

        public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PostgresOptions>(configuration.GetRequiredSection(SectionName));
            var options = configuration.GetOptions<PostgresOptions>(SectionName);

            services.AddDbContext<MySpotDbContext>(x => x.UseNpgsql(options.ConnectionString));
            services.AddScoped<IWeeklyParkingSpotRepository,PostgresWeeklyParkingSpotRepository>();
            services.AddScoped<IUserRepository,PostgresUserRepository>();
            services.AddScoped<IUnitOfWork,PostgresUnitOfWork>();
            services.TryDecorate(typeof(ICommandHandler<>), typeof(UnitOfWorkCommandHandlerDecorator<>));
            services.AddHostedService<DatabaseInitializer>();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior",true);

            return services;
        }
    }
}

