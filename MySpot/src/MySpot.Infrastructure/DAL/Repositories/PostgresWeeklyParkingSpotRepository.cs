﻿using Microsoft.EntityFrameworkCore;
using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;

namespace MySpot.Infrastructure.DAL.Repositories
{
    internal sealed class PostgresWeeklyParkingSpotRepository : IWeeklyParkingSpotRepository
    {
        private readonly MySpotDbContext _dbContext;

        public PostgresWeeklyParkingSpotRepository(MySpotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<WeeklyParkingSpot> GetAsync(ParkingSpotId id)
            => _dbContext.WeeklyParkingSpots
                .Include(x =>x.Reservations)
                .SingleOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<WeeklyParkingSpot>> GetAllAsync()
        {
            var res = await _dbContext.WeeklyParkingSpots
               .Include(x => x.Reservations)
               .ToListAsync();

            return res.AsEnumerable();
        }

        public async Task<IEnumerable<WeeklyParkingSpot>> GetByWeekAsync(Week week)
        => await _dbContext.WeeklyParkingSpots
                .Include(x => x.Reservations)
                .Where(x => x.Week == week)
                .ToListAsync();

        public async Task AddAsync(WeeklyParkingSpot weeklyParkingSpot)
        {
            await _dbContext.AddAsync(weeklyParkingSpot);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(WeeklyParkingSpot weeklyParkingSpot)
        {
            _dbContext.Update(weeklyParkingSpot);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(WeeklyParkingSpot weeklyParkingSpot)
        {
            _dbContext.Remove(weeklyParkingSpot);
            await _dbContext.SaveChangesAsync();
        }

        

        

        
    }
}
