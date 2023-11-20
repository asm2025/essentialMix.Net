using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using essentialMix.Data.Entity.Patterns.Repository;
using essentialMix.Data.Model;
using essentialMix.Extensions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using SystemDbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace essentialMix.Data.Entity.AutoMapper.Patterns.Services;

public abstract class Service<TContext, TRepository, TEntity, TKey> : ServiceBase<TContext, TRepository, TEntity, TKey>, IService<TContext, TRepository, TEntity, TKey>
	where TContext : SystemDbContext
	where TRepository : IRepository<TContext, TEntity, TKey>
	where TEntity : class, IEntity
{
	protected Service([NotNull] TRepository repository, [NotNull] IMapper mapper, [NotNull] ILogger logger)
		: base(repository, mapper, logger)
	{
	}

	/// <inheritdoc />
	public virtual TEntity Add(TEntity entity)
	{
		ThrowIfDisposed();
		entity = Repository.Add(entity);
		if (entity == null) return default(TEntity);
		Context.SaveChanges();
		return entity;
	}

	/// <inheritdoc />
	public virtual T Add<T>(TEntity entity)
	{
		entity = Add(entity);
		return entity == null
					? default(T)
					: Mapper.Map<T>(entity);
	}

	/// <inheritdoc />
	public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken token = default(CancellationToken))
	{
		ThrowIfDisposed();
		token.ThrowIfCancellationRequested();
		entity = await Repository.AddAsync(entity, token)
								.ConfigureAwait(false);
		token.ThrowIfCancellationRequested();
		if (entity == null) return default(TEntity);
		await Context.SaveChangesAsync(token).ConfigureAwait();
		return entity;
	}

	/// <inheritdoc />
	public virtual async Task<T> AddAsync<T>(TEntity entity, CancellationToken token = default(CancellationToken))
	{
		entity = await AddAsync(entity, token);
		token.ThrowIfCancellationRequested();
		return entity == null
					? default(T)
					: Mapper.Map<T>(entity);
	}

	/// <inheritdoc />
	public virtual TEntity Update(TEntity entity)
	{
		ThrowIfDisposed();
		entity = Repository.Update(entity);
		if (entity == null) return default(TEntity);
		Context.SaveChanges();
		return entity;
	}

	/// <inheritdoc />
	public virtual T Update<T>(TEntity entity)
	{
		entity = Update(entity);
		return entity == null
					? default(T)
					: Mapper.Map<T>(entity);
	}

	/// <inheritdoc />
	public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken token = default(CancellationToken))
	{
		ThrowIfDisposed();
		token.ThrowIfCancellationRequested();
		entity = await Repository.UpdateAsync(entity, token);
		token.ThrowIfCancellationRequested();
		if (entity == null) return default(TEntity);
		await Context.SaveChangesAsync(token);
		return entity;
	}

	/// <inheritdoc />
	public virtual async Task<T> UpdateAsync<T>(TEntity entity, CancellationToken token = default(CancellationToken))
	{
		ThrowIfDisposed();
		token.ThrowIfCancellationRequested();
		entity = await UpdateAsync(entity, token);
		token.ThrowIfCancellationRequested();
		return entity == null
					? default(T)
					: Mapper.Map<T>(entity);
	}

	/// <inheritdoc />
	public virtual TEntity Delete(TKey key)
	{
		ThrowIfDisposed();
		TEntity entity = Repository.Delete(key);
		if (entity == null) return default(TEntity);
		Context.SaveChanges();
		return entity;
	}

	/// <inheritdoc />
	public virtual T Delete<T>(TKey key)
	{
		ThrowIfDisposed();
		TEntity entity = Delete(key);
		return entity == null
					? default(T)
					: Mapper.Map<T>(entity);
	}

	/// <inheritdoc />
	public virtual async Task<TEntity> DeleteAsync(TKey key, CancellationToken token = default(CancellationToken))
	{
		ThrowIfDisposed();
		token.ThrowIfCancellationRequested();
		TEntity entity = await Repository.DeleteAsync(key, token);
		token.ThrowIfCancellationRequested();
		if (entity == null) return default(TEntity);
		await Context.SaveChangesAsync(token)
					.ConfigureAwait();
		return entity;
	}

	/// <inheritdoc />
	public virtual async Task<T> DeleteAsync<T>(TKey key, CancellationToken token = default(CancellationToken))
	{
		TEntity entity = await DeleteAsync(key, token);
		token.ThrowIfCancellationRequested();
		return entity == null
					? default(T)
					: Mapper.Map<T>(entity);
	}

	/// <inheritdoc />
	public virtual TEntity Delete(TEntity entity)
	{
		ThrowIfDisposed();
		entity = Repository.Delete(entity);
		if (entity == null) return default(TEntity);
		Context.SaveChanges();
		return entity;
	}

	/// <inheritdoc />
	public virtual T Delete<T>(TEntity entity)
	{
		ThrowIfDisposed();
		entity = Delete(entity);
		return entity == null
					? default(T)
					: Mapper.Map<T>(entity);
	}

	/// <inheritdoc />
	public virtual async Task<TEntity> DeleteAsync(TEntity entity, CancellationToken token = default(CancellationToken))
	{
		ThrowIfDisposed();
		token.ThrowIfCancellationRequested();
		entity = await Repository.DeleteAsync(entity, token);
		token.ThrowIfCancellationRequested();
		if (entity == null) return default(TEntity);
		await Context.SaveChangesAsync(token)
					.ConfigureAwait();
		return entity;
	}

	/// <inheritdoc />
	public virtual async Task<T> DeleteAsync<T>(TEntity entity, CancellationToken token = default(CancellationToken))
	{
		entity = await DeleteAsync(entity, token);
		token.ThrowIfCancellationRequested();
		return entity == null
					? default(T)
					: Mapper.Map<T>(entity);
	}
}