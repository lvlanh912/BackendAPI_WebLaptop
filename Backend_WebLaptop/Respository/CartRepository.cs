using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class CartRepository : ICartRepository
    {
        private readonly IMongoCollection<Cart> Carts;
        private readonly IAccountRepository Accounts;
        public CartRepository(IDatabase_Service database_Service, IAccountRepository accounts)
        {
            Carts = database_Service.Get_Carts_Collection();
            Accounts = accounts;
        }

        public async Task<bool> AddtoCart(CartItem cartItem,string accountId)
        {
            //các thay đổi
            var curentCart = await GetCart(accountId);
            UpdateDefinition<Cart> update;
            if (curentCart == null)
                throw new Exception("Account is does not exits");
            //thêm sản phẩm vào giỏ hàng khi giỏ hàng chưa có sản phẩm đó
            if(!Isexits_Item(cartItem,curentCart.Items!))
                update = Builders<Cart>.Update.Push<CartItem>(e => e.Items, cartItem);
            //nếu có sản phẩm rồi thì tăng số lượng-->chỉnh sửa giỏ hàng cũ 
            else
            {
                curentCart.Items!.Find(e=>e.ProductId==cartItem.ProductId)!.Quantity+=cartItem.Quantity;
                update = Builders<Cart>.Update.Set(e => e.Items, curentCart.Items);
            }
            var a = await Carts.UpdateOneAsync(filter: e => e.AccountID == accountId, update);
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

        public async Task<bool> Create(string userId)
        {
            if (!await Accounts.Exits(userId))
                throw new Exception("UserId not found?");
            await Carts.InsertOneAsync(new Cart { AccountID = userId,Items=new List<CartItem>() });
            return true;
        }

        public async Task<bool> DeleteItem(CartItem Item_remove,string userId)
        {
            var curentCart=await GetCart(userId);
            if (curentCart == null)
                throw new Exception("Account is does not exits");
            //nếu giỏ hàng trống 
            
            foreach(var item in curentCart.Items)
            {
                //tìm sản phẩm cần xoá trong danh sách sản phẩm
                if(Item_remove.ProductId == item.ProductId)
                {
                    //nếu sau khi xoá sản phẩm đó có số lượng bằng hoặc bé hơn 0 thì xoá khỏi giỏ hàng
                    if(item.Quantity<= Item_remove.Quantity)
                        curentCart.Items.Remove(item);
                    //trường hợp còn lại chỉ cần thay đổi số lượng của sản phẩm trong giỏ hàng bằng số hiện tại trừ số truyền vào
                    else
                    {
                        item.Quantity -= Item_remove.Quantity;
                    }
                }
            }
            var update = Builders<Cart>.Update.Set(e=>e.Items,curentCart.Items);
            var rs = await Carts.UpdateOneAsync(e => e.AccountID == userId, update);
           return rs.ModifiedCount > 0;
        }

        public async Task<bool> EmptyCart(string userID)
        {
            //thay items= danh sách trống
            var update=Builders<Cart>.Update.Set(e=>e.Items, new List<CartItem>());
            var rs =  await Carts.UpdateOneAsync(e => e.AccountID == userID, update);
            return rs.ModifiedCount > 0;
        }

        public async Task<Cart> GetCart(string userID)=>
            await  Carts.FindSync(e => e.AccountID == userID).FirstOrDefaultAsync();
    }
}
