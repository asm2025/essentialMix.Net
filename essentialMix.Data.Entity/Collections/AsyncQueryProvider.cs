using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;

namespace essentialMix.Data.Entity.Collections;

internal class AsyncQueryProvider<T> : IAsyncQueryProvider
{
	private readonly IQueryProvider _inner;

	internal AsyncQueryProvider(IQueryProvider inner)
	{
		_inner = inner;
	}

	[NotNull]
	public IQueryable CreateQuery(Expression expression)
	{
		return new AsyncQueryable<T>(expression);
	}

	[NotNull]
	public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
	{
		return new AsyncQueryable<TElement>(expression);
	}

	public object Execute(Expression expression)
	{
		return _inner.Execute(expression);
	}

	public TResult Execute<TResult>(Expression expression)
	{
		return _inner.Execute<TResult>(expression);
	}

	[NotNull]
	public IAsyncEnumerable<TResult> ExecuteAsync<TResult>([NotNull] Expression expression)
	{
		return new AsyncQueryable<TResult>(expression);
	}

	TResult IAsyncQueryProvider.ExecuteAsync<TResult>([NotNull] Expression expression, CancellationToken cancellationToken)
	{
		return Execute<TResult>(expression);
	}
}