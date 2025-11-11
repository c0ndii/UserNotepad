namespace UserNotepad.Models
{
    public class PageDto<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public required int TotalCount {  get; set; }
        public required int Page { get; set; }
        public required int PageSize { get; set; }
    }
}
