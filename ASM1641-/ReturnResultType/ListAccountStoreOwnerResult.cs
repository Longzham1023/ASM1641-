using System;
using ASM1641_.Models;

namespace ASM1641_.ReturnResultType
{
	public class ListAccountStoreOwnerResult
	{
        public int page { get; set; }
        public int totalPages { get; set; }
        public List<StoreOwner>? listAccounts { get; set; }
    }
}

