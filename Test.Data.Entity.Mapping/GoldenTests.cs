using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using essentialMix.Patterns.Sorting;
using NUnit.Framework;
using Test.Data.Entity.Mapping.Mapping;
using Test.Data.Entity.Mapping.Model;

namespace Test.Data.Entity.Mapping;

/// <summary>
/// Golden-master tests: each case runs a service call that maps through the mapper (in memory or as a query
/// projection) and compares the JSON result with a snapshot recorded under the previous mapping library.
/// </summary>
[TestFixture]
public class GoldenTests
{
	/// <summary>
	/// Every source/destination pair that has golden cases below. <see cref="EveryRegisteredMapHasAGoldenCase" />
	/// fails when a registered map is missing from this list.
	/// </summary>
	public static readonly IReadOnlyList<(Type Source, Type Destination)> CoveredPairs =
	[
		(typeof(Author), typeof(AuthorDto)),
		(typeof(Book), typeof(BookDto)),
		(typeof(Author), typeof(AuthorSummary)),
		(typeof(Author), typeof(AuthorStub))
	];

	private SampleDatabase _database;
	private SampleDbContext _context;

	[SetUp]
	public void SetUp()
	{
		_database = new SampleDatabase();
		_context = _database.NewContext();
	}

	[TearDown]
	public void TearDown()
	{
		_context.Dispose();
		_database.Dispose();
	}

	private AuthorService NewAuthorService() { return new AuthorService(new AuthorRepository(_context), TestMapper.Create()); }

	private BookService NewBookService() { return new BookService(new BookRepository(_context), TestMapper.Create()); }

	private static GetSettings IncludeAll() { return new GetSettings { Include = ["Books", "Address"] }; }

	[Test]
	public void EveryRegisteredMapHasAGoldenCase()
	{
		string[] registered = TestMapper.RegisteredPairs()
										.Select(e => $"{e.Source.FullName} -> {e.Destination.FullName}")
										.OrderBy(e => e)
										.ToArray();
		string[] covered = CoveredPairs.Select(e => $"{e.Source.FullName} -> {e.Destination.FullName}")
										.OrderBy(e => e)
										.ToArray();
		Assert.That(registered, Is.EquivalentTo(covered));
	}

	// In-memory maps (IMapper.Map) through Get<T>.

	[TestCase(SampleDatabase.FULL_AUTHOR_ID, TestName = "Get_AuthorDto_Full_NoInclude")]
	[TestCase(SampleDatabase.SPARSE_AUTHOR_ID, TestName = "Get_AuthorDto_Sparse_NoInclude")]
	[TestCase(SampleDatabase.NULL_NAME_AUTHOR_ID, TestName = "Get_AuthorDto_NullName_NoInclude")]
	[TestCase(SampleDatabase.MISSING_AUTHOR_ID, TestName = "Get_AuthorDto_Missing_NoInclude")]
	public void Get_AuthorDto_NoInclude(int id)
	{
		Snapshot.Match(TestContext.CurrentContext.Test.Name, NewAuthorService().Get<AuthorDto>(id));
	}

	[TestCase(SampleDatabase.FULL_AUTHOR_ID, TestName = "Get_AuthorDto_Full_Include")]
	[TestCase(SampleDatabase.SPARSE_AUTHOR_ID, TestName = "Get_AuthorDto_Sparse_Include")]
	[TestCase(SampleDatabase.NULL_NAME_AUTHOR_ID, TestName = "Get_AuthorDto_NullName_Include")]
	public void Get_AuthorDto_Include(int id)
	{
		Snapshot.Match(TestContext.CurrentContext.Test.Name, NewAuthorService().Get<AuthorDto>(id, IncludeAll()));
	}

	[TestCase(SampleDatabase.FULL_AUTHOR_ID, TestName = "GetAsync_AuthorDto_Full_Include")]
	[TestCase(SampleDatabase.SPARSE_AUTHOR_ID, TestName = "GetAsync_AuthorDto_Sparse_NoInclude")]
	public async Task GetAsync_AuthorDto(int id)
	{
		AuthorDto result = id == SampleDatabase.FULL_AUTHOR_ID
								? await NewAuthorService().GetAsync<AuthorDto>(id, IncludeAll())
								: await NewAuthorService().GetAsync<AuthorDto>(id);
		Snapshot.Match(TestContext.CurrentContext.Test.Name, result);
	}

	[TestCase(SampleDatabase.FULL_AUTHOR_ID, TestName = "Get_AuthorSummary_Full")]
	[TestCase(SampleDatabase.NULL_NAME_AUTHOR_ID, TestName = "Get_AuthorSummary_NullName")]
	public void Get_AuthorSummary(int id)
	{
		Snapshot.Match(TestContext.CurrentContext.Test.Name, NewAuthorService().Get<AuthorSummary>(id));
	}

	[Test]
	public void Get_AuthorStub()
	{
		Snapshot.Match(nameof(Get_AuthorStub), NewAuthorService().Get<AuthorStub>(SampleDatabase.FULL_AUTHOR_ID, IncludeAll()));
	}

	[TestCase(1, TestName = "Get_BookDto_NoInclude")]
	public void Get_BookDto_NoInclude(int id)
	{
		Snapshot.Match(TestContext.CurrentContext.Test.Name, NewBookService().Get<BookDto>(id));
	}

	[TestCase(1, TestName = "Get_BookDto_Include")]
	[TestCase(3, TestName = "Get_BookDto_NullTitle_Include")]
	public void Get_BookDto_Include(int id)
	{
		Snapshot.Match(TestContext.CurrentContext.Test.Name, NewBookService().Get<BookDto>(id, new GetSettings { Include = ["Author"] }));
	}

	// In-memory maps through the write methods of Service.

	[Test]
	public void Add_BookDto()
	{
		Book book = new Book { Id = 10, Title = "New", Price = 5m, AuthorId = SampleDatabase.SPARSE_AUTHOR_ID };
		Snapshot.Match(nameof(Add_BookDto), NewBookService().Add<BookDto>(book));
	}

	[Test]
	public async Task AddAsync_AuthorDto()
	{
		Author author = new Author { Id = 10, Name = "Grace", Born = new DateTime(1906, 12, 9) };
		Snapshot.Match(nameof(AddAsync_AuthorDto), await NewAuthorService().AddAsync<AuthorDto>(author));
	}

	[Test]
	public void Update_AuthorDto()
	{
		AuthorService service = NewAuthorService();
		Author author = service.Get(SampleDatabase.FULL_AUTHOR_ID, IncludeAll());
		author.Name = "Ada Lovelace";
		Snapshot.Match(nameof(Update_AuthorDto), service.Update<AuthorDto>(author));
	}

	[Test]
	public async Task UpdateAsync_BookDto()
	{
		BookService service = NewBookService();
		Book book = service.Get(2);
		book.Price = 31m;
		Snapshot.Match(nameof(UpdateAsync_BookDto), await service.UpdateAsync<BookDto>(book));
	}

	[Test]
	public void Delete_BookDto_ByKey()
	{
		Snapshot.Match(nameof(Delete_BookDto_ByKey), NewBookService().Delete<BookDto>(3));
	}

	[Test]
	public async Task DeleteAsync_BookDto_ByEntity()
	{
		BookService service = NewBookService();
		Book book = service.Get(3, new GetSettings { Include = ["Author"] });
		Snapshot.Match(nameof(DeleteAsync_BookDto_ByEntity), await service.DeleteAsync<BookDto>(book));
	}

	// Query projections through List<T>.

	[Test]
	public void List_AuthorDto_All()
	{
		Snapshot.Match(nameof(List_AuthorDto_All), NewAuthorService().List<AuthorDto>());
	}

	[Test]
	public void List_AuthorDto_SortedPaged()
	{
		ListSettings settings = new ListSettings
		{
			Page = 1,
			PageSize = 2,
			OrderBy = [new SortField("Born", SortType.Descending)]
		};
		Snapshot.Match(nameof(List_AuthorDto_SortedPaged), NewAuthorService().List<AuthorDto>(settings));
	}

	[Test]
	public async Task ListAsync_AuthorDto_Filtered()
	{
		ListSettings settings = new ListSettings
		{
			FilterExpression = "Id != 2",
			OrderBy = [new SortField("Id")]
		};
		Snapshot.Match(nameof(ListAsync_AuthorDto_Filtered), await NewAuthorService().ListAsync<AuthorDto>(settings));
	}

	[Test]
	public void List_AuthorSummary_All()
	{
		Snapshot.Match(nameof(List_AuthorSummary_All), NewAuthorService().List<AuthorSummary>());
	}

	[Test]
	public async Task ListAsync_BookDto_All()
	{
		Snapshot.Match(nameof(ListAsync_BookDto_All), await NewBookService().ListAsync<BookDto>());
	}
}
