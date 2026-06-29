using GymMangmentSystem.DAL.Data.DbContexts;
using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        public ISessionRepository SessionRepository {get;}

        private readonly GymDbContext _dbContext;
        private readonly Dictionary<string, object> repostories = [];
        public UnitOfWork( GymDbContext dbContext, ISessionRepository sessionRepository)
        {
            _dbContext = dbContext;
            SessionRepository = sessionRepository;
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var typeName = typeof(TEntity).Name;
            if (repostories.TryGetValue(typeName, out var repostory))
            {
                return (IGenericRepository<TEntity>)repostory;
            }

            var repo = new GenericRepository<TEntity>(_dbContext);
            repostories[typeName] = repo;
            return repo;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _dbContext.SaveChangesAsync(ct);
        }
    }
}
