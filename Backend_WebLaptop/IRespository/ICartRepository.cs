using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface ICartRepository
    {
        //tạo giỏ hàng mới
        Task<bool> Create(string userId);
        //lấy giỏ hàng
        Task<Cart> GetCart(string userID);
        //làm trống giỏ hàng
        Task<bool> EmptyCart(string userID);
        //thêm sản phẩm vào giỏ
        Task<bool> AddtoCart(CartItem cartItem, string userId);
        //xoá sản phẩm vào giỏ hàng
        Task<bool> DeleteItem(CartItem cartItem, string userId);
    }
}
