using System;
using ASM1641_.Models;

namespace ASM1641_.Dtos
{
	public class AuthorResult
	{
		public int page { get; set; }
		public int totalPages { get; set; }
		public List<Author>? authors { get; set; }
	}
}

