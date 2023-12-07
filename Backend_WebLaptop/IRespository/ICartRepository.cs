using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface ICartRepository
    {
        //tạo giỏ hàng mới
        Task<Cart> Create(string userId);
        //lấy giỏ hàng
        Task<Cart> GetCart(string userId);
        //làm trống giỏ hàng
        Task<bool> EmptyCart(string userId);
        //thêm sản phẩm vào giỏ
        Task<bool> AddtoCart(CartItem cartItem, string userId);
        //xoá giỏ hàng
        Task DeleteCart(string userId);
        //Cập nhật giỏ hàng, 1 item
        /// <summary>
        /// Update Item in Cart
        /// isdelete --delete item in cart
        /// !isdelete -- update quantity
        /// </summary>
        /// <returns></returns>
        Task UpdateCart(CartItem cartItem,string AccountId,bool isDelete);
    }
}
