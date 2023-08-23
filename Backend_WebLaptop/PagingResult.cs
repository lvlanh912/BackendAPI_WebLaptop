namespace Backend_WebLaptop
{
    public class PagingResult<T>
    {
        public IEnumerable<T>? Items { get; set; }
        public int TotalCount =>Items!=null? Items.Count():0;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}