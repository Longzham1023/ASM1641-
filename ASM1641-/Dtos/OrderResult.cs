using System;
using ASM1641_.Models;

namespace ASM1641_.Dtos
{
	public class OrderResult
	{
		public int page { get; set; }
		public int totalPages { get; set; }
		public List<Orders>? orders { get; set; }
		public long totalOrders { get; set; }
	}
}

