using System;
using MongoDB.Driver;
using ASM1641_.Models;
using Microsoft.Extensions.Options;
using ASM1641_.Data;
using ASM1641_.IService;
using ASM1641_.Dtos;

namespace ASM1641_.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly IMongoCollection<Category> _categoriesCollection;
		private readonly IOptions<DatabaseSetting> _dbSettings;

		public CategoryService(IOptions<DatabaseSetting> dbSettings)
		{
			this._dbSettings = dbSettings;
			var mongoClient = new MongoClient(this._dbSettings.Value.ConnectionString);
			var mongoDatabase = mongoClient.GetDatabase(this._dbSettings.Value.DatabaseName);

			_categoriesCollection = mongoDatabase.GetCollection<Category>(this._dbSettings.Value.CategoriesCollection);
		}

        public async Task CreateCategory(Category aCategory)
        {
            await _categoriesCollection.InsertOneAsync(aCategory);
        }

       
        public async Task<CateResult> GetAllCategories( int pageNumber)
        {
            if (pageNumber <= 0)
            {
                throw new ArgumentException("pageNumber must be greater than zero.");
            }

            int skip = (pageNumber - 1) * 10;

            var totalCates = await _categoriesCollection.CountDocumentsAsync(_ => true);
            var totalPages = (int)Math.Ceiling((double)totalCates / 10);

            var cates = await _categoriesCollection.Find(_ => true).Skip(skip).Limit(10).ToListAsync();


            return new CateResult
            {
                page = pageNumber,
                totalPages = totalPages,
                categories = cates
            };
        }

        public async Task<Category> GetByID(string id)
        {
            return await _categoriesCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

      

        public async Task RemoveCategory(string Id)
        {
            await _categoriesCollection.DeleteOneAsync(e => e.Id == Id);
        }
       
        public async Task UpdateCategory(string name, string categoryId)
        {
            var categoryFilter = Builders<Category>.Filter.Eq(e => e.Id, categoryId);
            var updateDefinition = Builders<Category>.Update.Set(e => e.Name, name);

            await _categoriesCollection.UpdateOneAsync(categoryFilter, updateDefinition);
        }

      
    }
}

