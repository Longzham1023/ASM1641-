using System;
using MongoDB.Driver;
using ASM1641_.Models;
using Microsoft.Extensions.Options;
using ASM1641_.Data;
using ASM1641_.IService;

namespace BookStore.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly IMongoCollection<Category> _CategoriesCollection;
		private readonly IOptions<DatabaseSetting> _dbSettings;

		public CategoryService(IOptions<DatabaseSetting> dbSettings)
		{
			this._dbSettings = dbSettings;
			var mongoClient = new MongoClient(this._dbSettings.Value.ConnectionString);
			var mongoDatabase = mongoClient.GetDatabase(this._dbSettings.Value.DatabaseName);

			_CategoriesCollection = mongoDatabase.GetCollection<Category>(this._dbSettings.Value.CategoriesCollection);
		}

        public async Task CreateCategory(Category aCategory)
        {
            await _CategoriesCollection.InsertOneAsync(aCategory);
        }

        public async Task<IEnumerable<Category>> GetAllCategories(int pageSize, int pageNumber)
        {
            int skip = (pageNumber - 1) * pageSize;
            return await _CategoriesCollection.Find(_ => true)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync();
        }

        public Task<IEnumerable<Category>> GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public async Task<Category> GetByID(string id)
        {
            return await _CategoriesCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

      

        public async Task RemoveCategory(string Id)
        {
            await _CategoriesCollection.DeleteOneAsync(e => e.Id == Id);
        }
       
        public async Task UpdateCategory(string name, string categoryId)
        {
            var categoryFilter = Builders<Category>.Filter.Eq(e => e.Id, categoryId);
            var updateDefinition = Builders<Category>.Update.Set(e => e.Name, name);

            await _CategoriesCollection.UpdateOneAsync(categoryFilter, updateDefinition);
        }

    }
}

