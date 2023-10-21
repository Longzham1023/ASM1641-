using ASM1641_.Models;

namespace ASM1641_.Dtos
{
    public class CateResult
    {
        public int page { get; set; }
        public int totalPages { get; set; }
        public List<Category>? categories { get; set; }
    }
}
