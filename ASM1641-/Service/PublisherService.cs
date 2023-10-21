using ASM1641_.Data;
using ASM1641_.IService;
using ASM1641_.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ASM1641_.Service
{
    public class PublisherService : IPublisherService
    {
        private readonly IMongoCollection<Publisher> _publisherCollection;
        private readonly IOptions<DatabaseSetting> _dbSetting;

        public PublisherService(IOptions<DatabaseSetting> dbSetting)
        {
            this._dbSetting = dbSetting;
            var mongoClient = new MongoClient(this._dbSetting.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(this._dbSetting.Value.DatabaseName);

            _publisherCollection = mongoDatabase.GetCollection<Publisher>(this._dbSetting.Value.PublishersCollection);
        }

        public async Task CreatePublisher(Publisher aPublisher)
        {
            var publisher = await _publisherCollection.Find(e => true).ToListAsync();

            foreach (var e in publisher)
            {
                if (aPublisher.Name.Equals(e.Name))
                {
                    throw new Exception("Name of publisher has been created!");
                }
            }

            await _publisherCollection.InsertOneAsync(aPublisher);
        }
        public async Task<Publisher> GetByID(string id)
            => await _publisherCollection.Find(e => e.Id == id).FirstOrDefaultAsync();

        public async Task<IEnumerable<Publisher>> GetPublishers()
            => await _publisherCollection.Find(e => true).ToListAsync();

        public async Task RemovePublisher(string Id)
            => await _publisherCollection.DeleteOneAsync(e => e.Id == Id);

        public async Task UpdatePublisher(Publisher aPublisher, string Id)
        {
            var publisherFilter = Builders<Publisher>.Filter.Eq("Id", Id);
            var publisher = await _publisherCollection.Find(publisherFilter).FirstOrDefaultAsync();

            if (publisherFilter != null)
            {
                if (publisher != null)
                {
                    publisher.Name = aPublisher.Name;
                    publisher.Address = aPublisher.Address;
                    publisher.Email = aPublisher.Email;
                    publisher.Phone = aPublisher.Phone;
                    await _publisherCollection.ReplaceOneAsync(publisherFilter, publisher);
                }
            }
        }
    }
}
