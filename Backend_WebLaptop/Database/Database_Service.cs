using Backend_WebLaptop.Configs;
using Backend_WebLaptop.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Backend_WebLaptop.Database

{
    public class Database_Service : IDatabase_Service
    {
        private readonly IMongoCollection<Account> _accounts;
        private readonly IMongoCollection<District> _districts;
        private readonly IMongoCollection<Province> _provinces;
        private readonly IMongoCollection<Ward> _wards;

        public Database_Service(IOptions<DatabaseConfig> _Databaseconfig)
        {
            var mongoClient = new MongoClient(_Databaseconfig.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(_Databaseconfig.Value.DatabaseName);
            _accounts = mongoDatabase.GetCollection<Account>(_Databaseconfig.Value.Accounts_Collections);
            _provinces = mongoDatabase.GetCollection<Province>(_Databaseconfig.Value.Provinces_Collections);
            _districts = mongoDatabase.GetCollection<District>(_Databaseconfig.Value.Districts_Collections);
            _wards = mongoDatabase.GetCollection<Ward>(_Databaseconfig.Value.Wards_Collections);
        }
        public IMongoCollection<Account> Get_Accounts_Collection()
        {
            return _accounts;
        }

        public IMongoCollection<District> Get_District_Collection()
        {
            return _districts;
        }

        public IMongoCollection<Province> Get_Provinces_Collection()
        {
            return _provinces;
        }

        public IMongoCollection<Ward> Get_Ward_Collection()
        {
            return _wards;
        }
    }
}
