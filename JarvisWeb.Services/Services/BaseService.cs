using JarvisWeb.Domain;
using JarvisWeb.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Services
{
    public abstract class BaseService<T>(
        JarvisWebDbContext context,
        ILogger logger,
        DbSet<T> repository) where T : UserData
    {
        protected readonly JarvisWebDbContext _context = context;
        protected readonly ILogger _logger = logger;
        protected readonly DbSet<T> _repository = repository;

        public virtual async Task<T?> GetByIdAsync(Guid userId, Guid id)
        {
            _logger.LogInformation($"Getting {typeof(T).Name} with id {id}");
            return await _repository.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == id);
        }

        public virtual async Task<T> Create(T entity)
        {
            _logger.LogInformation($"Creating {typeof(T).Name} for user {entity.UserId}");
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }
            _repository.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> Update(T entity)
        {
            _logger.LogInformation($"Updating {typeof(T).Name} with id {entity.Id}");
            _repository.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task Delete(Guid userId, Guid id)
        {
            _logger.LogInformation($"Deleting {typeof(T).Name} with id {id}");
            var entity = await _repository.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == id);
            _repository.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual IQueryable<T> GetEntries(Guid userId)
        {
            _logger.LogInformation($"Getting all {typeof(T).Name} for user {userId}");
            return _repository.Where(x => x.UserId == userId);
        }
    }
}
