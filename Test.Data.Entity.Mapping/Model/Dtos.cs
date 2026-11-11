using System;
using System.Collections.Generic;

namespace Test.Data.Entity.Mapping.Model;

public class AuthorDto
{
	public int Id { get; set; }
	public string Name { get; set; }
	public DateTime Born { get; set; }
	// Flattened from Address.City by naming convention.
	public string AddressCity { get; set; }
	// Custom member expression.
	public int BookCount { get; set; }
	public List<BookDto> Books { get; set; }
	// No source member: explicitly ignored.
	public string Nickname { get; set; }
}

public class BookDto
{
	public int Id { get; set; }
	public string Title { get; set; }
	public decimal Price { get; set; }
	// Flattened from Author.Name by naming convention.
	public string AuthorName { get; set; }
}

public class AuthorSummary
{
	public int Id { get; set; }
	public string DisplayName { get; set; }
}

// Mapped with IgnoreAll(): every member keeps its default value.
public class AuthorStub
{
	public int Id { get; set; }
	public string Name { get; set; }
	public DateTime Born { get; set; }
}
