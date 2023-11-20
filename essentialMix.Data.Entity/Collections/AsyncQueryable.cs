using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace essentialMix.Data.Entity.Collections;

internal class AsyncQueryable<T> : EnumerableQuery<T>, IQueryable<T>, IAsyncEnumerable<T>
{
	public AsyncQueryable([NotNull] IEnumerable<T> enumerable)
		: base(enumerable)
	{
	}

	public AsyncQueryable([NotNull] Expression expression)
		: base(expression)
	{
	}

	IQueryProvider IQueryable.Provider => new AsyncQueryProvider<T>(this);

	[NotNull]
	public IAsyncEnumerator<T> GetEnumerator() { return new AsyncEnumerator(this.AsEnumerable().GetEnumerator()); }

	[NotNull]
	public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) { return new AsyncEnumerator(this.AsEnumerable().GetEnumerator()); }

	internal class AsyncEnumerator : IAsyncEnumerator<T>
	{
		private readonly IEnumerator<T> _inner;

		public AsyncEnumerator(IEnumerator<T> inner)
		{
			_inner = inner;
		}

		public T Current => _inner.Current;

		public ValueTask<bool> MoveNextAsync()
		{
			return new ValueTask<bool>(_inner.MoveNext());
		}

		public ValueTask DisposeAsync()
		{
			_inner.Dispose();
			return new ValueTask();
		}
	}
}