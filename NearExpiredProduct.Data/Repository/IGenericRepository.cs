﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Data.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetWhere(Expression<Func<T, bool>>? filter = null);
        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null);
        DbSet<T> GetAll();
        Task CreateAsync(T entity);
        EntityEntry<T> Delete(T entity);
        IQueryable<T> FindAll(Func<T, bool> predicate);
        T Find(Func<T, bool> predicate);
        Task<T> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetById(int id);
        Task Update(T entity, int Id);

    }
}
