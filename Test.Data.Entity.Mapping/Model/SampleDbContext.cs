using System;
using System.Collections.Generic;
using essentialMix.Data.Entity.Patterns.Repository;
using JetBrains.Annotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using SystemDbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace Test.Data.Entity.Mapping.Model;

public class SampleDbContext(DbContextOptions<SampleDbContext> options) : SystemDbContext(options)
{
	public DbSet<Author> Authors { get; set; }
	public DbSet<Book> Books { get; set; }
	public DbSet<Address> Addresses { get; set; }
}

public class AuthorRepository([NotNull] SampleDbContext context)
	: Repository<SampleDbContext, Author, int>(context, new ConfigurationBuilder().Build(), NullLogger.Instance);

public class BookRepository([NotNull] SampleDbContext context)
	: Repository<SampleDbContext, Book, int>(context, new ConfigurationBuilder().Build(), NullLogger.Instance);

/// <summary>
/// An in-memory SQLite database seeded with a fixed data set. Each <see cref="NewContext" /> call returns a fresh
/// context, so nothing is tracked from seeding and navigation properties stay unloaded unless included.
/// </summary>
public sealed class SampleDatabase : IDisposable
{
	public const int FULL_AUTHOR_ID = 1;
	public const int SPARSE_AUTHOR_ID = 2;
	public const int NULL_NAME_AUTHOR_ID = 3;
	public const int MISSING_AUTHOR_ID = 999;

	private readonly SqliteConnection _connection;

	public SampleDatabase()
	{
		_connection = new SqliteConnection("DataSource=:memory:");
		_connection.Open();

		using (SampleDbContext context = NewContext())
		{
			context.Database.EnsureCreated();
			context.Addresses.AddRange(new Address { Id = 1, City = "London", Street = "12 St James's Square" }, new Address { Id = 2, City = "Paris", Street = "1 Rue de Rivoli" });
			context.Authors.AddRange(new Author
			{
				Id = FULL_AUTHOR_ID,
				Name = "Ada",
				Born = new DateTime(1815, 12, 10),
				AddressId = 1,
				Books = new List<Book>
				{
					new Book { Id = 1, Title = "Notes", Price = 12.5m },
					new Book { Id = 2, Title = "Sketch of the Analytical Engine", Price = 30m }
				}
			}, new Author
			{
				Id = SPARSE_AUTHOR_ID,
				Name = "Brian",
				Born = new DateTime(1942, 1, 1)
			}, new Author
			{
				Id = NULL_NAME_AUTHOR_ID,
				Name = null,
				Born = new DateTime(2000, 2, 29),
				AddressId = 2,
				Books = new List<Book>
				{
					new Book { Id = 3, Title = null, Price = 0.99m }
				}
			});
			context.SaveChanges();
		}
	}

	[NotNull]
	public SampleDbContext NewContext(bool lazyLoadingProxies = false)
	{
		DbContextOptionsBuilder<SampleDbContext> builder = new DbContextOptionsBuilder<SampleDbContext>().UseSqlite(_connection);
		if (lazyLoadingProxies) builder.UseLazyLoadingProxies();
		return new SampleDbContext(builder.Options);
	}

	public void Dispose() { _connection.Dispose(); }
}
