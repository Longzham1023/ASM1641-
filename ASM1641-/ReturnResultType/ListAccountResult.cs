using System;
using ASM1641_.Models;

namespace ASM1641_.ReturnResultType
{
	public class ListAccountResult
	{
		public int page { get; set; }
		public int totalPages { get; set; }
		public List<Customer>? listAccounts { get; set; }
	}
}

