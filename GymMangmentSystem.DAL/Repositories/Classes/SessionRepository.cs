using GymMangmentSystem.DAL.Data.DbContexts;
using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _dbContext ;
        public SessionRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(CancellationToken ct)
        {
            var sessions = _dbContext.Sessions.Include(S=>S.Category).Include(S=>S.Trainer);

            return await sessions.ToListAsync(ct);
        }

        public Task<int> GetCountOfBokkedSlotsAsync(int sessionId, CancellationToken ct = default)
        {
            return _dbContext.Bookings.CountAsync(B=> B.SessionId == sessionId , ct);
        }

        public async Task<Session?> GetSessionWithTrainerAndCategoryAsync(int sessionId, CancellationToken ct = default)
        {
            return await _dbContext.Sessions.AsNoTracking()
                .Include(S => S.Category)
                .Include(S => S.Trainer)
                .FirstOrDefaultAsync(S => S.Id == sessionId, ct);
        }
    }
}
