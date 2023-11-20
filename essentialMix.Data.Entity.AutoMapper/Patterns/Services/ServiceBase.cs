using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using essentialMix.Data.Entity.Patterns.Repository;
using essentialMix.Data.Model;
using essentialMix.Data.Patterns.Parameters;
using essentialMix.Extensions;
using essentialMix.Patterns.Object;
using essentialMix.Patterns.Pagination;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SystemDbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace essentialMix.Data.Entity.AutoMapper.Patterns.Services;

public abstract class ServiceBase<TContext> : Disposable, IServiceBase<TContext>
	where TContext : SystemDbContext
{
	protected ServiceBase([NotNull] TContext context, [NotNull] IMapper mapper, [NotNull] ILogger logger)
	{
		Context = context;
		Mapper = mapper;
		Logger = logger;
	}

	/// <inheritdoc />
	public TContext Context { get; }

	/// <inheritdoc />
	public IMapper Mapper { get; }

	/// <inheritdoc />
	public ILogger Logger { get; }
}

public abstract class ServiceBase<TContext, TRepository, TEntity, TKey> : Disposable, IServiceBase<TContext, TRepository, TEntity, TKey>
	where TContext : SystemDbContext
	where TRepository : IRepositoryBase<TContext, TEntity, TKey>
	where TEntity : class, IEntity
{
	protected ServiceBase([NotNull] TRepository repository, [NotNull] IMapper mapper, [NotNull] ILogger logger)
	{
		Repository = repository;
		Mapper = mapper;
		Logger = logger;
	}

	/// <inheritdoc />
	public Type EntityType { get; } = typeof(TEntity);

	/// <inheritdoc />
	public TRepository Repository { get; }

	/// <inheritdoc />
	public TContext Context => Repository.Context;

	/// <inheritdoc />
	public IMapper Mapper { get; }

	/// <inheritdoc />
	public ILogger Logger { get; }

	/// <inheritdoc />
	[NotNull]
	public IPaginated<TEntity> List(IPagination settings = null) { return List(Repository.List(settings), settings); }
	/// <inheritdoc />
	[NotNull]
	public IPaginated<T> List<T>(IPagination settings = null) { return List<T>(Repository.List(settings), settings); }
	/// <inheritdoc />
	public virtual IPaginated<TEntity> List(IQueryable<TEntity> queryable, IPagination settings = null)
	{
		ThrowIfDisposed();

		if (settings is { PageSize: > 0 })
		{
			settings.Count = PrepareCountQuery(queryable, settings).Count();
			int maxPages = (int)Math.Ceiling(settings.Count / (double)settings.PageSize);
			if (settings.Page > maxPages) settings.Page = maxPages;
		}

		queryable = PrepareListQuery(queryable, settings);
		IList<TEntity> result = queryable.ToList();
		return new Paginated<TEntity>(result, settings);
	}

	/// <inheritdoc />
	public virtual IPaginated<T> List<T>(IQueryable<TEntity> queryable, IPagination settings = null)
	{
		ThrowIfDisposed();

		if (settings is { PageSize: > 0 })
		{
			settings.Count = PrepareCountQuery(queryable, settings).Count();
			int maxPages = (int)Math.Ceiling(settings.Count / (double)settings.PageSize);
			if (settings.Page > maxPages) settings.Page = maxPages;
		}

		queryable = PrepareListQuery(queryable, settings);
		IList<T> result = queryable.ProjectTo<T>(Mapper.ConfigurationProvider)
									.ToList();
		return new Paginated<T>(result, settings);
	}

	/// <inheritdoc />
	[NotNull]
	public Task<IPaginated<TEntity>> ListAsync(CancellationToken token = default(CancellationToken)) { return ListAsync(Repository.List(), null, token); }
	/// <inheritdoc />
	[NotNull]
	public Task<IPaginated<TEntity>> ListAsync(IPagination settings, CancellationToken token = default(CancellationToken)) { return ListAsync(Repository.List(settings), settings, token); }

	/// <inheritdoc />
	[NotNull]
	public Task<IPaginated<T>> ListAsync<T>(CancellationToken token = default(CancellationToken)) { return ListAsync<T>(Repository.List(), null, token); }

	/// <inheritdoc />
	public Task<IPaginated<TEntity>> ListAsync(IQueryable<TEntity> queryable, CancellationToken token = default(CancellationToken))
	{
		ThrowIfDisposed();
		token.ThrowIfCancellationRequested();
		return ListAsync(queryable, null, token);
	}

	/// <inheritdoc />
	public async Task<IPaginated<TEntity>> ListAsync(IQueryable<TEntity> queryable, IPagination settings, CancellationToken token = default(CancellationToken))
	{
		ThrowIfDisposed();
		token.ThrowIfCancellationRequested();

		if (settings is { PageSize: > 0 })
		{
			settings.Count = await PrepareCountQuery(queryable, settings)
									.CountAsync(token)
									.ConfigureAwait();
			token.ThrowIfCancellationRequested();
			int maxPages = (int)Math.Ceiling(settings.Count / (double)settings.PageSize);
			if (settings.Page > maxPages) settings.Page = maxPages;
		}

		queryable = PrepareListQuery(queryable, settings);
		IList<TEntity> result = await queryable.ToListAsync(token)
												.ConfigureAwait();
		token.ThrowIfCancellationRequested();
		return new Paginated<TEntity>(result, settings);
	}

	/// <inheritdoc />
	[NotNull]
	[ItemNotNull]
	public Task<IPaginated<T>> ListAsync<T>(IPagination settings, CancellationToken token = default(CancellationToken)) { return ListAsync<T>(Repository.List(settings), settings, token); }

	/// <inheritdoc />
	public Task<IPaginated<T>> ListAsync<T>(IQueryable<TEntity> queryable, CancellationToken token = default(CancellationToken))
	{
		ThrowIfDisposed();
		token.ThrowIfCancellationRequested();
		return ListAsync<T>(queryable, null, token);
	}

	/// <inheritdoc />
	public virtual async Task<IPaginated<T>> ListAsync<T>(IQueryable<TEntity> queryable, IPagination settings, CancellationToken token = default(CancellationToken))
	{
		ThrowIfDisposed();
		token.ThrowIfCancellationRequested();

		if (settings is { PageSize: > 0 })
		{
			settings.Count = await PrepareCountQuery(queryable, settings)
								.CountAsync(token)
								.ConfigureAwait();
			token.ThrowIfCancellationRequested();
			int maxPages = (int)Math.Ceiling(settings.Count / (double)settings.PageSize);
			if (settings.Page > maxPages) settings.Page = maxPages;
		}

		queryable = PrepareListQuery(queryable, settings);
		IList<T> result = await queryable.ProjectTo<T>(Mapper.ConfigurationProvider)
										.ToListAsync(token)
										.ConfigureAwait();
		token.ThrowIfCancellationRequested();
		return new Paginated<T>(result, settings);
	}

	/// <inheritdoc />
	public virtual TEntity Get(TKey key)
	{
		ThrowIfDisposed();
		TEntity entity = Repository.Get(key);
		return entity;
	}

	/// <inheritdoc />
	public virtual TEntity Get(TKey key, IGetSettings settings)
	{
		ThrowIfDisposed();
		TEntity entity = Repository.Get(key, settings);
		return entity;
	}

	/// <inheritdoc />
	public virtual T Get<T>(TKey key)
	{
		ThrowIfDisposed();
		TEntity entity = Get(key);
		return entity == null
				? default(T)
				: Mapper.Map<T>(entity);
	}

	/// <inheritdoc />
	public virtual T Get<T>(TKey key, IGetSettings settings)
	{
		ThrowIfDisposed();
		TEntity entity = Get(key, settings);
		return entity == null
					? default(T)
					: Mapper.Map<T>(entity);
	}

	/// <inheritdoc />
	public virtual async Task<TEntity> GetAsync(TKey key, CancellationToken token = default(CancellationToken))
	{
		ThrowIfDisposed();
		token.ThrowIfCancellationRequested();
		TEntity entity = await Repository.GetAsync(key, token)
										.ConfigureAwait();
		token.ThrowIfCancellationRequested();
		return entity;
	}

	/// <inheritdoc />
	public virtual async Task<TEntity> GetAsync(TKey key, IGetSettings settings, CancellationToken token = default(CancellationToken))
	{
		ThrowIfDisposed();
		token.ThrowIfCancellationRequested();
		TEntity entity = await Repository.GetAsync(key, settings, token)
										.ConfigureAwait();
		token.ThrowIfCancellationRequested();
		return entity;
	}

	/// <inheritdoc />
	public virtual async Task<T> GetAsync<T>(TKey key, CancellationToken token = default(CancellationToken))
	{
		ThrowIfDisposed();
		token.ThrowIfCancellationRequested();
		TEntity entity = await GetAsync(key, token)
										.ConfigureAwait();
		token.ThrowIfCancellationRequested();
		return entity == null
					? default(T)
					: Mapper.Map<T>(entity);
	}

	/// <inheritdoc />
	public virtual async Task<T> GetAsync<T>(TKey key, IGetSettings settings, CancellationToken token = default(CancellationToken))
	{
		ThrowIfDisposed();
		token.ThrowIfCancellationRequested();
		TEntity entity = await GetAsync(key, settings, token)
										.ConfigureAwait();
		token.ThrowIfCancellationRequested();
		return entity == null
					? default(T)
					: Mapper.Map<T>(entity);
	}

	[NotNull]
	protected virtual IQueryable<TEntity> PrepareListQuery([NotNull] IQueryable<TEntity> queryable, IPagination settings)
	{
		if (settings is not { PageSize: > 0 }) return queryable;
		queryable = queryable.Paginate(settings);
		return queryable;
	}

	[NotNull]
	protected virtual IQueryable<TEntity> PrepareCountQuery([NotNull] IQueryable<TEntity> queryable, IPagination settings)
	{
		return queryable;
	}
}