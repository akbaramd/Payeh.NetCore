namespace Payeh.SharedKernel.EntityFrameworkCore.FluentQueries;

/// <summary>
/// Represents a sorting criterion for a query.
/// </summary>
public class FluentQuerySort
{
    public string Field { get; set; }
    public string Direction { get; set; } = "asc"; // Default to ascending
}