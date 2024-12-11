namespace Payeh.FluentQuery
{
    public class FilterItem
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }

    public class SortItem
    {
        public string Field { get; set; }
        public string Direction { get; set; }
    }

    public  partial class FluentFilterQuery
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public List<FilterItem> Filters { get; set; }
        public List<SortItem> Sorts { get; set; }
    }
}