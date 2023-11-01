using System;
using ASM1641_.Dtos;
using ASM1641_.ReturnResultType;

namespace ASM1641_.IService
{
	public interface IAdminService
	{
		Task<ListAccountResult> ViewListAccountUser(int page);
		Task<ListAccountStoreOwnerResult> ViewListAccountStoreOwners(int page);
		Task DeleteUserAccount(string userId);
		Task CreateStoreOwnerAccount(StoreOwnerDto storeOwnerDto);
	}
}

