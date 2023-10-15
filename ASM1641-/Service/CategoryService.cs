using System;
using MongoDB.Driver;
using BookStore.Models;
using Microsoft.Extensions.Options;
using BookStore.Data;
using BookStore.IServices;

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
            => await _CategoriesCollection.InsertOneAsync(aCategory);

        public async Task<IEnumerable<Category>> GetAllCategories()
            => await _CategoriesCollection.FindSync(e => true).ToListAsync();

        public async Task<Category> GetByID(string id)
            => await _CategoriesCollection.FindSync(e => e.Id == id).FirstOrDefaultAsync();

        public async Task RemoveCategory(string Id)
            => await _CategoriesCollection.FindOneAndDeleteAsync(e => e.Id == Id);

        public async Task UpdateCategory(string name, string categoryId)
        {
            var categoryFilter = Builders<Category>.Filter.Eq("Id", categoryId);
            var category = await _CategoriesCollection.Find(categoryFilter).FirstOrDefaultAsync();

            if (category != null)
            {
                category.Name = name;
                await _CategoriesCollection.ReplaceOneAsync(categoryFilter, category);
            }
        }

    }
}

