﻿using Microsoft.EntityFrameworkCore;
using MySpot.Core.Entities;
using MySpot.Core.ValueObjects;
using MySpot.Infrastructure.DAL.Configurations;

namespace MySpot.Infrastructure.DAL
{
    public sealed class MySpotDbContext : DbContext
    {
        public DbSet<Reservation> Reservations{get;set;}
        public DbSet<WeeklyParkingSpot> WeeklyParkingSpots{get;set;}
        public DbSet<User> Users {get;set;}

        public MySpotDbContext(DbContextOptions<MySpotDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<UserId>();
            modelBuilder.ApplyConfiguration(new CleaningReservationConfiguration());
            modelBuilder.ApplyConfiguration(new VehicleReservationConfiguration());
            modelBuilder.ApplyConfiguration(new ReservationConfiguration());
            modelBuilder.ApplyConfiguration(new WeeklyParkingSpotConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }

    }
}
