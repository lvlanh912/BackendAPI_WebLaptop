using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetAll();
        Task<Payment> GetbyId(string id);
        Task<bool> DeletebyId(string id);
        Task<Payment> Insert(Payment entity);
        Task<bool> Update(Payment entity);
        Task<bool> Exits(string id);
    }
}
