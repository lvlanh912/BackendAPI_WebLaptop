using Backend_WebLaptop.Model;
using MongoDB.Driver;

namespace Backend_WebLaptop.Database
{
    public interface IDatabaseService
    {
        IMongoCollection<Account> Get_Accounts_Collection();
        IMongoCollection<Province> Get_Provinces_Collection();
        IMongoCollection<District> Get_District_Collection();
        IMongoCollection<Ward> Get_Ward_Collection();
        IMongoCollection<ShippingAddress> Get_ShippingAddress_Collection();
        IMongoCollection<Product> Get_Products_Collection();
        IMongoCollection<Category> Get_Categories_Collection();
        IMongoCollection<Comment> Get_Comments_Collection();
        IMongoCollection<Cart> Get_Carts_Collection();
        IMongoCollection<Voucher> Get_Vouchers_Collection();
        IMongoCollection<Order> Get_Orders_Collection();
        IMongoCollection<Payment> Get_Payments_Collections();
        IMongoCollection<News> Get_News_Collections();

    }
}
