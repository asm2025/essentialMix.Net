using AutoMapper;
using essentialMix.Extensions;
using Test.Data.Entity.Mapping.Model;

namespace Test.Data.Entity.Mapping.Mapping;

public class SampleProfile : Profile
{
	public SampleProfile()
	{
		CreateMap<Author, AuthorDto>()
			.ForMember(d => d.BookCount, o => o.MapFrom(s => s.Books.Count))
			.ForMember(d => d.Nickname, o => o.Ignore());
		CreateMap<Book, BookDto>();
		CreateMap<Author, AuthorSummary>()
			.ForMember(d => d.DisplayName, o => o.MapFrom(s => s.Name + " (" + s.Born.Year + ")"));
		CreateMap<Author, AuthorStub>()
			.IgnoreAll();
	}
}
