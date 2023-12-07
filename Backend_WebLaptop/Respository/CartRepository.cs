using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class CartRepository : ICartRepository
    {
        private readonly IMongoCollection<Cart> _carts;
        public CartRepository(IDatabaseService databaseService)
        {
            _carts = databaseService.Get_Carts_Collection();
        }

        public async Task<bool> AddtoCart(CartItem cartItem, string accountId)
        {
            //các thay đổi
            var curentCart = await GetCart(accountId);
            UpdateDefinition<Cart> update;
            if (curentCart == null)
                throw new Exception("Account is does not exist");
            //thêm sản phẩm vào giỏ hàng khi giỏ hàng chưa có sản phẩm đó
            if (!Isexits_Item(cartItem, curentCart.Items!))
                update = Builders<Cart>.Update.Push<CartItem>(e => e.Items, cartItem);
            //nếu có sản phẩm rồi thì tăng số lượng-->chỉnh sửa giỏ hàng cũ 
            else
            {
                curentCart.Items!.Find(e => e.ProductId == cartItem.ProductId)!.Quantity += cartItem.Quantity;
                update = Builders<Cart>.Update.Set(e => e.Items, curentCart.Items);
            }
            var a = await _carts.UpdateOneAsync(filter: e => e.AccountId == accountId, update);
            return a.ModifiedCount > 0;
        }

        static bool Isexits_Item(CartItem comming, List<CartItem> stores)
        {
            foreach (var item in stores)
            {
                if (comming.ProductId == item.ProductId)
                    return true;
            }
            return false;
        }

        public async Task<Cart> Create(string userId)
        {
            var cart = new Cart { AccountId = userId, Items = new List<CartItem>() };
            await _carts.InsertOneAsync(cart);
            return cart;
        }



        public async Task<bool> EmptyCart(string userId)
        {
            //thay items= danh sách trống
            var update = Builders<Cart>.Update.Set(e => e.Items, new List<CartItem>());
            var rs = await _carts.UpdateOneAsync(e => e.AccountId == userId, update);
            return rs.ModifiedCount > 0;
        }
        public async Task<Cart> GetCart(string userId){
            return await _carts.FindSync(e => e.AccountId == userId).FirstOrDefaultAsync() ?? await Create(userId);
        }
        
        //xoá giỏ hàng trong database của user
        public async Task DeleteCart(string userId) =>
          await _carts.DeleteOneAsync(e => e.AccountId == userId);

        public async Task UpdateCart(CartItem cartItem,string AccountId,bool isDelete)
        {
            //Thay đổi số lượng sản phẩm
            if (!isDelete)
            {
                //validate data
                if (cartItem.Quantity <= 0)
                    throw new Exception("Không thể cập nhật số lượng");
                var filterItem = Builders<Cart>.Filter.And(
                     Builders<Cart>.Filter.Eq(e => e.AccountId, AccountId),
                     Builders<Cart>.Filter.ElemMatch(e => e.Items, Builders<CartItem>.Filter.Eq(e => e.ProductId, cartItem.ProductId))
                     );
                var update = Builders<Cart>.Update.Set("Items.$.Quantity", cartItem.Quantity);
                await _carts.UpdateOneAsync(filterItem, update);
            }
            else
            {
                var filterItem = Builders<Cart>.Filter.And(
                     Builders<Cart>.Filter.Eq(e => e.AccountId, AccountId),
                     Builders<Cart>.Filter.ElemMatch(e => e.Items, Builders<CartItem>.Filter.Eq(e => e.ProductId, cartItem.ProductId))
                     );
                 var update = Builders<Cart>.Update.PullFilter(e=>e.Items, Builders<CartItem>.Filter.Eq(e=>e.ProductId, cartItem.ProductId));
                await _carts.UpdateOneAsync(filterItem,update);
            }
                   
            
          
        }
    }
}
