using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using essentialMix.Data.Entity.Patterns.Repository;
using essentialMix.Data.Model;
using essentialMix.Data.Patterns.Service;
using essentialMix.Patterns.Pagination;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using SystemDbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace essentialMix.Data.Entity.Patterns.Service;

public interface IServiceBase<out TContext> : IServiceBase
	where TContext : SystemDbContext
{
	[NotNull]
	TContext Context { get; }
	[NotNull]
	ILogger Logger { get; }
}

public interface IServiceBase<out TContext, out TRepository, TEntity, TKey> : IServiceBase<TEntity, TKey>
	where TContext : SystemDbContext
	where TRepository : IRepositoryBase<TContext, TEntity, TKey>
	where TEntity : class, IEntity
{
	[NotNull]
	TRepository Repository { get; }
	[NotNull]
	TContext Context { get; }
	[NotNull]
	ILogger Logger { get; }
	IPaginated<TEntity> List([NotNull] IQueryable<TEntity> queryable, IPagination settings = null);
	IPaginated<T> List<T>([NotNull] IQueryable<TEntity> queryable, IPagination settings = null);
	[NotNull]
	Task<IPaginated<TEntity>> ListAsync([NotNull] IQueryable<TEntity> queryable, CancellationToken token = default(CancellationToken));
	[NotNull]
	Task<IPaginated<TEntity>> ListAsync([NotNull] IQueryable<TEntity> queryable, IPagination settings, CancellationToken token = default(CancellationToken));
	[NotNull]
	Task<IPaginated<T>> ListAsync<T>([NotNull] IQueryable<TEntity> queryable, CancellationToken token = default(CancellationToken));
	[NotNull]
	Task<IPaginated<T>> ListAsync<T>([NotNull] IQueryable<TEntity> queryable, IPagination settings, CancellationToken token = default(CancellationToken));
}