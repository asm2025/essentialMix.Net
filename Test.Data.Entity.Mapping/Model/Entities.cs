using System;
using System.Collections.Generic;
using essentialMix.Data.Model;

namespace Test.Data.Entity.Mapping.Model;

// Navigation properties are virtual so the same model works with EF lazy-loading proxies.
public class Author : IEntity<int>
{
	public int Id { get; set; }
	public string Name { get; set; }
	public DateTime Born { get; set; }
	public int? AddressId { get; set; }
	public virtual Address Address { get; set; }
	public virtual ICollection<Book> Books { get; set; }
}

public class Book : IEntity<int>
{
	public int Id { get; set; }
	public string Title { get; set; }
	public decimal Price { get; set; }
	public int AuthorId { get; set; }
	public virtual Author Author { get; set; }
}

public class Address : IEntity<int>
{
	public int Id { get; set; }
	public string City { get; set; }
	public string Street { get; set; }
}
