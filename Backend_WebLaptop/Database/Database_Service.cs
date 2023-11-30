using Backend_WebLaptop.Configs;
using Backend_WebLaptop.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Backend_WebLaptop.Database

{
    public class DatabaseService : IDatabaseService
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
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<Payment> _payments;
        private readonly IMongoCollection<News> _news;
        private readonly IMongoCollection<Session> _sessions;
        private readonly IMongoCollection<Chat> _chats;
        public DatabaseService(IOptions<DatabaseConfig> databaseconfig)
        {
            var mongoClient = new MongoClient(databaseconfig.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseconfig.Value.DatabaseName);
            _accounts = mongoDatabase.GetCollection<Account>(databaseconfig.Value.AccountsCollections);
            _provinces = mongoDatabase.GetCollection<Province>(databaseconfig.Value.ProvincesCollections);
            _districts = mongoDatabase.GetCollection<District>(databaseconfig.Value.DistrictsCollections);
            _wards = mongoDatabase.GetCollection<Ward>(databaseconfig.Value.WardsCollections);
            _shippingAddress = mongoDatabase.GetCollection<ShippingAddress>(databaseconfig.Value.ShippingAddressCollections);
            _products = mongoDatabase.GetCollection<Product>(databaseconfig.Value.ProductsCollections);
            _categories = mongoDatabase.GetCollection<Category>(databaseconfig.Value.CategoryCollections);
            _comments = mongoDatabase.GetCollection<Comment>(databaseconfig.Value.CommentsCollections);
            _carts = mongoDatabase.GetCollection<Cart>(databaseconfig.Value.CartsCollections);
            _vouchers = mongoDatabase.GetCollection<Voucher>(databaseconfig.Value.VouchersCollections);
            _orders = mongoDatabase.GetCollection<Order>(databaseconfig.Value.OrdersCollections);
            _payments = mongoDatabase.GetCollection<Payment>(databaseconfig.Value.PaymentsCollections);
            _news= mongoDatabase.GetCollection<News>(databaseconfig.Value.NewsCollections);
            _sessions = mongoDatabase.GetCollection<Session>(databaseconfig.Value.SessionsCollections);
            _chats = mongoDatabase.GetCollection<Chat>(databaseconfig.Value.ChatsCollections);
        }

        public IMongoCollection<Account> Get_Accounts_Collection() => _accounts;
        public IMongoCollection<District> Get_District_Collection() => _districts;
        public IMongoCollection<Province> Get_Provinces_Collection() => _provinces;
        public IMongoCollection<Ward> Get_Ward_Collection() => _wards;
        public IMongoCollection<ShippingAddress> Get_ShippingAddress_Collection() => _shippingAddress;
        public IMongoCollection<Product> Get_Products_Collection() => _products;
        public IMongoCollection<Category> Get_Categories_Collection() => _categories;
        public IMongoCollection<Comment> Get_Comments_Collection() => _comments;
        public IMongoCollection<Cart> Get_Carts_Collection() => _carts;
        public IMongoCollection<Voucher> Get_Vouchers_Collection() => _vouchers;
        public IMongoCollection<Payment> Get_Payments_Collections() => _payments;
        public IMongoCollection<Order> Get_Orders_Collection() => _orders;
        public IMongoCollection<News> Get_News_Collections() => _news;
        public IMongoCollection<Session> Get_Sessions_Collections() => _sessions;
        public IMongoCollection<Chat> Get_Chats_Collections() => _chats;
        
    }
}
