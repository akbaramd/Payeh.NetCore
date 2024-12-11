namespace Payeh.FluentQuery;

public  class EntityFrameworkFluentFilterQuery : FluentFilterQuery
{
    public string[] Includes { get; set; } = [];
}