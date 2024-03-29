﻿namespace Auto3D.Data.Common.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        IQueryable<TEntity> All();

        IQueryable<TEntity> AllAsNoTracking();

        Task AddAsync(TEntity entity);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void UpdateRange(ICollection<TEntity> entities);

        void Delete(TEntity entity);

        Task<int> SaveChangesAsync();
    }
}
