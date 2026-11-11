using System.Collections.Generic;
using essentialMix.Data.Patterns.Parameters;
using essentialMix.Patterns.Pagination;

namespace Test.Data.Entity.Mapping;

public class GetSettings : IGetSettings
{
	public IList<string> Include { get; set; }
	public string FilterExpression { get; set; }
}

public class ListSettings : SortablePagination, IIncludeSettings, IFilterSettings
{
	public IList<string> Include { get; set; }
	public string FilterExpression { get; set; }
}
