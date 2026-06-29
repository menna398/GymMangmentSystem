using GymMangmentSystem.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(CancellationToken ct = default);
        Task<int> GetCountOfBokkedSlotsAsync(int sessionId,CancellationToken ct = default);
        Task<Session> GetSessionWithTrainerAndCategoryAsync( int sessionId,CancellationToken ct = default);
    }
}
