using System.Data.Common;
using System.Linq.Expressions;
using JadehRo.Database.Context;
using JadehRo.Database.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace JadehRo.Database.Repositories;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
    AppDbContext AppDbContext { get; }

    DbConnection DbConnection { get; }

    DbSet<TEntity> Entities { get; }
    IQueryable<TEntity> Table { get; }
    IQueryable<TEntity> TableNoTracking { get; }

    void Add(TEntity entity, bool saveNow = false, bool ignorePreSaveChange = false);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = false, bool ignorePreSaveChange = false);
    void AddRange(IEnumerable<TEntity> entities, bool saveNow = false, bool ignorePreSaveChange = false);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = false, bool ignorePreSaveChange = false);
    void Attach(TEntity entity);
    void Delete(TEntity entity, bool saveNow = false, bool ignorePreSaveChange = false);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = false, bool ignorePreSaveChange = false);
    void DeleteRange(IEnumerable<TEntity> entities, bool saveNow = false, bool ignorePreSaveChange = false);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = false, bool ignorePreSaveChange = false);
    void Detach(TEntity entity);
    TEntity Find(params object[] ids);
    TEntity FindAsNoTracking(params object[] ids);
    ValueTask<TEntity> GetByIdAsync(CancellationToken cancellationToken, params object[] ids);
    void LoadCollection<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty) where TProperty : class;
    void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty) where TProperty : class;
    void Update(TEntity entity, bool saveNow = false, bool ignorePreSaveChange = false);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = false, bool ignorePreSaveChange = false);
    void UpdateRange(IEnumerable<TEntity> entities, bool saveNow = false, bool ignorePreSaveChange = false);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = false, bool ignorePreSaveChange = false);

    IQueryable<T> RunQuery<T>(string sql, params object[] ids) where T : class;
    IQueryable<TEntity> RunQuery(string sql, params object[] ids);
    void Save();
}