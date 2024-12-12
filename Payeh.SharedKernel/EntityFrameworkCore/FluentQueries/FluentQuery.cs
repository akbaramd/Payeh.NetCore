namespace Payeh.SharedKernel.EntityFrameworkCore.FluentQueries;

/// <summary>
/// Represents the full set of filters, sorting, paging, and includes for a query.
/// </summary>
public class FluentQuery
{
    public int Take { get; set; } = 10; // Default page size
    public int Skip { get; set; } = 0; // Default page index
    public List<FluentQueryFilter> Filters { get; set; } = new();
    public List<FluentQuerySort> Sorts { get; set; } = new();
    public string[] Includes { get; set; } = Array.Empty<string>();
}