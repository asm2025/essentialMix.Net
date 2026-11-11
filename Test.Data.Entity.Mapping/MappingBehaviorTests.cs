using System;
using System.Collections.Generic;
using essentialMix.Data.Entity.Mapster;
using Mapster;
using MapsterMapper;
using NUnit.Framework;
using Test.Data.Entity.Mapping.Mapping;
using Test.Data.Entity.Mapping.Model;

namespace Test.Data.Entity.Mapping;

/// <summary>
/// Behaviors the golden snapshots cannot show on their own.
/// </summary>
[TestFixture]
public class MappingBehaviorTests
{
	[Test]
	public void Configuration_Compiles()
	{
		Assert.DoesNotThrow(TestMapper.Compile);
	}

	/// <summary>
	/// Known difference: the previous library mapped a null top-level collection to an empty list. Mapster returns
	/// null. The services never map a top-level collection, so only callers using <c>Mapper</c> directly are affected.
	/// </summary>
	[Test]
	public void NullTopLevelCollection_MapsToNull()
	{
		List<BookDto> result = TestMapper.Create().Map<List<BookDto>>(null);
		Assert.That(result, Is.Null);
	}

	/// <summary>
	/// Known difference: the previous library null-propagated custom member expressions such as <c>s.Books.Count</c>
	/// when mapping in memory. Mapster throws when the collection is null, so the expression needs an explicit null
	/// check (see SampleRegister). Plain member chains such as <c>s.Address.City</c> stay null-safe, and query
	/// projections are unaffected.
	/// </summary>
	[Test]
	public void MemberExpression_IsNotNullPropagated()
	{
		TypeAdapterConfig config = MappingConfiguration.Create();
		config.NewConfig<Author, AuthorSummary>()
				.Map(d => d.DisplayName, s => s.Books.Count.ToString());
		IMapper mapper = new Mapper(config);
		Assert.Throws<NullReferenceException>(() => mapper.Map<Author, AuthorSummary>(new Author { Books = null }));
	}

	[Test]
	public void NullCollectionMember_MapsToEmptyList()
	{
		Author author = new Author { Id = 1, Name = "Ada", Books = null };
		AuthorDto result = TestMapper.Create().Map<AuthorDto>(author);
		Assert.That(result.Books, Is.Not.Null.And.Empty);
		Assert.That(result.BookCount, Is.Zero);
	}

	[Test]
	public void LazyLoadingProxySource_MapsThroughTheEntityMap()
	{
		using SampleDatabase database = new SampleDatabase();
		using SampleDbContext context = database.NewContext(lazyLoadingProxies: true);
		AuthorService service = new AuthorService(new AuthorRepository(context), TestMapper.Create());
		Author entity = service.Get(SampleDatabase.FULL_AUTHOR_ID);
		Assert.That(entity.GetType(), Is.Not.EqualTo(typeof(Author)), "Expected an EF lazy-loading proxy.");
		Snapshot.Match("LazyLoadingProxy_Get_AuthorDto", service.Get<AuthorDto>(SampleDatabase.FULL_AUTHOR_ID));
	}

	/// <summary>
	/// Known difference: the previous library fell back to the base type's map for a derived or proxy runtime type.
	/// With explicit mapping required, Mapster matches the runtime type pair exactly, so <c>Map&lt;T&gt;(object)</c>
	/// throws for a proxy. The services avoid this by mapping with the static entity type
	/// (<c>Map&lt;TEntity, T&gt;</c>); callers using <c>Mapper</c> directly must do the same.
	/// </summary>
	[Test]
	public void LazyLoadingProxySource_ThrowsWhenMappedByRuntimeType()
	{
		using SampleDatabase database = new SampleDatabase();
		using SampleDbContext context = database.NewContext(lazyLoadingProxies: true);
		Author entity = context.Authors.Find(SampleDatabase.FULL_AUTHOR_ID);
		IMapper mapper = TestMapper.Create();
		Assert.That(entity!.GetType(), Is.Not.EqualTo(typeof(Author)), "Expected an EF lazy-loading proxy.");
		Assert.Throws<CompileException>(() => mapper.Map<AuthorDto>(entity));
		Assert.DoesNotThrow(() => mapper.Map<Author, AuthorDto>(entity));
	}
}
