using ASM1641_.Models;

namespace ASM1641_.Dtos
{
    public class BookResult
    {
        public int page { get; set; }
        public int totalPages { get; set; }
        public List<Book>? books { get; set; }
    }
}
