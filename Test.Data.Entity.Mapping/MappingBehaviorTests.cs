using System.Collections.Generic;
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
	public void Configuration_IsValid()
	{
		Assert.DoesNotThrow(TestMapper.AssertConfigurationIsValid);
	}

	[Test]
	public void NullTopLevelCollection()
	{
		List<BookDto> result = TestMapper.Create().Map<List<BookDto>>(null);
		Snapshot.Match(nameof(NullTopLevelCollection), result);
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
}
