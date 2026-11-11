using essentialMix.Extensions;
using Mapster;
using Test.Data.Entity.Mapping.Model;

namespace Test.Data.Entity.Mapping.Mapping;

public class SampleRegister : IRegister
{
	public void Register(TypeAdapterConfig config)
	{
		config.NewConfig<Author, AuthorDto>()
				// The previous library null-propagated member expressions in memory; Mapster does not.
				.Map(d => d.BookCount, s => s.Books == null ? 0 : s.Books.Count)
				.Ignore(d => d.Nickname);
		config.NewConfig<Book, BookDto>();
		config.NewConfig<Author, AuthorSummary>()
				.Map(d => d.DisplayName, s => s.Name + " (" + s.Born.Year + ")");
		config.NewConfig<Author, AuthorStub>()
				.IgnoreAll();
	}
}
