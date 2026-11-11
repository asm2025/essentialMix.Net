using essentialMix.Data.Entity.Mapster.Patterns.Services;
using JetBrains.Annotations;
using MapsterMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Test.Data.Entity.Mapping.Model;

namespace Test.Data.Entity.Mapping.Mapping;

public class AuthorService([NotNull] AuthorRepository repository, [NotNull] IMapper mapper)
	: Service<SampleDbContext, AuthorRepository, Author, int>(repository, mapper, NullLogger.Instance);

public class BookService([NotNull] BookRepository repository, [NotNull] IMapper mapper)
	: Service<SampleDbContext, BookRepository, Book, int>(repository, mapper, NullLogger.Instance);
