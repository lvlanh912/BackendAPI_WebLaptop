using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IShippingAddressRepository
    {
        Task<List<ShippingAddress>> GetAll(string AccountID);//get all shippng address by accountId
        Task<ShippingAddress> GetbyId(string id);
        Task<bool> DeletebyId(string id);
        Task<ShippingAddress> Insert(ShippingAddress entity);
        Task<bool> Update(ShippingAddress entity);
    }
}
