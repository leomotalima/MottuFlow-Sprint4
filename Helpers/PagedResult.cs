namespace MottuFlowApi.Helpers
{
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalCount { get; set; }
        public List<T> Items { get; set; } = new();
    }
}