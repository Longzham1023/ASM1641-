using System;
using Amazon.Runtime.Internal;
using ASM1641_.Data;
using ASM1641_.Dtos;
using ASM1641_.IService;
using ASM1641_.Models;
using ASM1641_.ReturnResultType;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ASM1641_.Service
{
    public class AdminService : IAdminService
	{
        private readonly IOptions<DatabaseSetting> _dbSetting;
		private readonly IMongoCollection<Customer> _customerCollection;
        private readonly IMongoCollection<StoreOwner> _storeOwnerCollection;

        public AdminService(IOptions<DatabaseSetting> dbSetting)
		{
			_dbSetting = dbSetting;
            var mongoClient = new MongoClient(this._dbSetting.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(this._dbSetting.Value.DatabaseName);

            _customerCollection = mongoDatabase.GetCollection<Customer>(this._dbSetting.Value.CustomerCollection);
            _storeOwnerCollection = mongoDatabase.GetCollection<StoreOwner>(this._dbSetting.Value.StoreOwnerCollection);

        }

        public async Task CreateStoreOwnerAccount(StoreOwnerDto storeOwnerDto)
        {
            var isExistEmailUser = await _customerCollection.Find(e => e.Email == storeOwnerDto.Email).FirstOrDefaultAsync();
            if(isExistEmailUser != null)
            {
                throw new Exception("Email has been existed!");
            }

            var isExistEmailStoreOwner = await _storeOwnerCollection.Find(e => e.Email == storeOwnerDto.Email).FirstOrDefaultAsync();
            if (isExistEmailStoreOwner != null)
            {
                throw new Exception("Email has been existed!");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(storeOwnerDto.Password);

            StoreOwner sto = new StoreOwner()
            {
                Email = storeOwnerDto.Email,
                PasswordHash = hashedPassword,
                Role = "StoreOwner"
            };

            await _storeOwnerCollection.InsertOneAsync(sto);
        }

        public async Task DeleteUserAccount(string userId)
        {
            var isExist = await _customerCollection.Find(e => e.Id == userId).FirstOrDefaultAsync();
            if(isExist == null)
            {
                throw new Exception("User not found!");
            }
            await _customerCollection.FindOneAndDeleteAsync(e => e.Id == isExist.Id);
        }

        public async Task<ListAccountStoreOwnerResult> ViewListAccountStoreOwners(int page)
        {
            if (page <= 0)
            {
                throw new Exception("Page must be greater than 0");
            }

            var sto = Builders<StoreOwner>.Filter.Eq("Role", "StoreOwner");

            int skip = (page - 1) * 10;

            var totalOwners = await _storeOwnerCollection.CountDocumentsAsync(sto);

            var totalPages = (int)Math.Ceiling((double)totalOwners / 10);

            var list = await _storeOwnerCollection.Find(sto).Skip(skip).Limit(10).ToListAsync();

            return new ListAccountStoreOwnerResult()
            {
                page = page,
                totalPages = totalPages,
                listAccounts = list
            };
        }

        public async Task<ListAccountResult> ViewListAccountUser(int page)
        {
            if(page <= 0)
            {
                throw new Exception("Page must be greater than 0");
            }

            var users = Builders<Customer>.Filter.Eq("Role", "User");

            int skip = (page - 1) * 10;

            var totalAuthors = await _customerCollection.CountDocumentsAsync(users);

            var totalPages = (int)Math.Ceiling((double)totalAuthors / 10);

            var list = await _customerCollection.Find(users).Skip(skip).Limit(10).ToListAsync();

            return new ListAccountResult()
            {
                page = page,
                totalPages = totalPages,
                listAccounts = list
            };
        }
    }
}

