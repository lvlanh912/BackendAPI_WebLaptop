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
        private readonly IMongoCollection<ShippingAddress> _shippingAddress;
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<Category> _categories;
        private readonly IMongoCollection<Comment> _comments;
        private readonly IMongoCollection<Cart> _carts;
        private readonly IMongoCollection<Voucher> _vouchers;

        public Database_Service(IOptions<DatabaseConfig> _Databaseconfig)
        {
            var mongoClient = new MongoClient(_Databaseconfig.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(_Databaseconfig.Value.DatabaseName);
            _accounts = mongoDatabase.GetCollection<Account>(_Databaseconfig.Value.Accounts_Collections);
            _provinces = mongoDatabase.GetCollection<Province>(_Databaseconfig.Value.Provinces_Collections);
            _districts = mongoDatabase.GetCollection<District>(_Databaseconfig.Value.Districts_Collections);
            _wards = mongoDatabase.GetCollection<Ward>(_Databaseconfig.Value.Wards_Collections);
            _shippingAddress= mongoDatabase.GetCollection<ShippingAddress>(_Databaseconfig.Value.Shipping_Address_Collections);
            _products = mongoDatabase.GetCollection<Product>(_Databaseconfig.Value.Products_Collections);
            _categories= mongoDatabase.GetCollection<Category>(_Databaseconfig.Value.Category_Collections);
            _comments = mongoDatabase.GetCollection<Comment>(_Databaseconfig.Value.Comments_Collections);
            _carts= mongoDatabase.GetCollection<Cart>(_Databaseconfig.Value.Carts_Collections);
            _vouchers = mongoDatabase.GetCollection<Voucher>(_Databaseconfig.Value.Vouchers_Collections);
        }

        public IMongoCollection<Account> Get_Accounts_Collection() => _accounts;
        public IMongoCollection<District> Get_District_Collection()=> _districts;
        public IMongoCollection<Province> Get_Provinces_Collection()=> _provinces;
        public IMongoCollection<Ward> Get_Ward_Collection()=> _wards;
        public IMongoCollection<ShippingAddress> Get_ShippingAddress_Collection()=> _shippingAddress;
        public IMongoCollection<Product> Get_Products_Collection()=> _products;
        public IMongoCollection<Category> Get_Categories_Collection()=> _categories;
        public IMongoCollection<Comment> Get_Comments_Collection()=> _comments;
        public IMongoCollection<Cart> Get_Carts_Collection() => _carts;
        public IMongoCollection<Voucher> Get_Vouchers_Collection() => _vouchers;
    }
}
