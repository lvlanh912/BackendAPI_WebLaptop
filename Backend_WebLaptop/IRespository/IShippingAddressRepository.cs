using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IShippingAddressRepository
    {
        Task<List<ShippingAddress>> GetAll(string accountId);//get all shippng address by accountId
        Task<ShippingAddress> GetbyId(string id);
        Task DeletebyId(string id,string accountId);
        Task<ShippingAddress> Insert(ShippingAddress entity);
        Task<bool> Update(ShippingAddress entity);
    }
}
