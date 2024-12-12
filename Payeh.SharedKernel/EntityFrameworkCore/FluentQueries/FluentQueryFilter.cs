namespace Payeh.SharedKernel.EntityFrameworkCore.FluentQueries;

/// <summary>
/// Represents a filter for a query.
/// </summary>
public class FluentQueryFilter
{
    public string Field { get; set; }
    public string Operator { get; set; }
    public string Value { get; set; }
}