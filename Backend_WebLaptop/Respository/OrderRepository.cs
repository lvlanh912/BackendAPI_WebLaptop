using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly IPaymentRepository _payments;
        private readonly IAccountRepository _accounts;
        private readonly IVoucherRepository _vouchers;
        private readonly IProductRepository _products;
        private readonly IShippingAddressRepository _shippingaddress;

        public OrderRepository(IDatabaseService databaseService, IPaymentRepository payments, IAccountRepository accounts
            , IVoucherRepository voucher, IProductRepository products, IShippingAddressRepository shippingaddress
            )
        {
            _orders = databaseService.Get_Orders_Collection();
            _payments = payments;
            _accounts = accounts;
            _vouchers = voucher;
            _products = products;
            _shippingaddress = shippingaddress;

        }

        /* public async Task<Order> Checkout(Order entity)
         {
             //nếu có voucher
             if (!string.IsNullOrWhiteSpace(entity.VoucherCode))
                 entity.Total = await DecreaseByVoucher(entity.Total, entity.VoucherCode);
             //kiểm tra số lượng sản phẩm xem có thể bán hay không
             if (await _products.Cansell(entity.Items!))
             return entity;
         }*/

        public async Task<PagingResult<Order>> GetAllOrders(string? userId, string? keywords, string? paymentId, int pageSize, int pageindex, int start, int end)
        {
            var startdate = DateTime.Now.AddDays(-start);
            var enddate = DateTime.Now.AddDays(-end);
            var filter = Builders<Order>.Filter;
            var builderFilter = filter.Empty;

            //&= là toán tử and
            if (userId != null)
                builderFilter &= filter.Eq(e => e.AccountId, userId);
            if (paymentId != null)
                builderFilter &= filter.Eq(e => e.PaymentMethodId, paymentId);
            if (keywords != null)
                builderFilter &= filter.Where(e => e.Status!.Last().Description!.Contains(keywords));

            builderFilter &= filter.And(filter.Gte(e => e.CreateAt, startdate), filter.Lte(e => e.CreateAt, enddate));

            var orders = await _orders.FindAsync(builderFilter);

            var list = await orders.ToListAsync();

            return new PagingResult<Order>
            {
                Items = list,
                PageIndex = pageindex,
                PageSize = pageSize
            };
        }


        public async Task<Order> CreateOrder(Order entity)
        {
            var shipping = new List<Shipping>{
                    new Shipping{Description="Đã tạo đơn hàng",UpdateAt=DateTime.Now}
                };
            entity.ShippingAddress = await _shippingaddress.GetbyId(entity.ShippingAddress!.Id!);
            if (await Validate(entity))
            {
                entity.Total = await Gettoltal(entity.Items!);
                entity.CreateAt = DateTime.Now;
                if (entity.PaymentMethodId != null)
                    shipping.Add(new Shipping { Description = "Đang chờ thanh toán", UpdateAt = DateTime.Now });
                entity.Status = shipping;
                entity.Paid = 0;

                //trường hợp có voucher
                if (entity.VoucherCode != null)
                    entity.Total = await DecreaseByVoucher(entity.Total, entity.VoucherCode);
                //kiểm tra số lượng sản phẩm và giảm số lượng sản phẩm và tăng số lượng bán.
                var t1 = _products.DecreaseQuantity(entity.Items!);
                var t2 = _orders.InsertOneAsync(entity);
                await Task.WhenAll(t1, t2);
            }
            return entity;

        }

        public async Task<bool> DeleteOrder(string id)
        {
            var rs = await _orders.DeleteOneAsync(e => e.Id == id);
            return rs.DeletedCount > 0;
        }

        public async Task<Order> EditOrder(Order entity, string id)
        {
            var curent = _orders.FindSync(e => e.Id == id);
            if (curent == null)
                throw new Exception("item does not exits");
            var filter = Builders<Order>.Filter.Eq(e => e.Id, id);
            var updatebuilder = new List<UpdateDefinition<Order>>();

            //tạo cac thay đổi
            if (entity.Status != null)
                updatebuilder.Add(Builders<Order>.Update.Set(e => e.Status, entity.Status));
            if (entity.Items != null)
                updatebuilder.Add(Builders<Order>.Update.Set(e => e.Items, entity.Items));
            if (entity.PaymentMethodId != null)
                updatebuilder.Add(Builders<Order>.Update.Set(e => e.PaymentMethodId, entity.PaymentMethodId));
            if (entity.ShippingAddress != null)
                updatebuilder.Add(Builders<Order>.Update.Set(e => e.ShippingAddress, entity.ShippingAddress));
            if (entity.Paid != null)
                updatebuilder.Add(Builders<Order>.Update.Set(e => e.Paid, entity.Paid));
            var update = Builders<Order>.Update.Combine(updatebuilder);
            return await _orders.FindOneAndUpdateAsync(filter, update);
        }



        public Task<Order> GetOrderbyCode(string voucherCode)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetOrderbyId(string voucherId)
        {
            throw new NotImplementedException();
        }

        public Task<long> Get_toltalSell(DateTime? start, DateTime? end)
        {
            throw new NotImplementedException();
        }

        public Task<Order> UpdateStatus(string id)
        {
            throw new NotImplementedException();
        }

        async Task<bool> Validate(Order entity)
        {
            //check voucher isvalid
            if (entity.VoucherCode != null && !await _vouchers.IsValidCode(entity.VoucherCode!))
            {
                throw new Exception("VoucherCode invalid");
            }
            bool[] arr = new bool[]
            {
              string.IsNullOrWhiteSpace(entity.PaymentMethodId) ||(!string.IsNullOrWhiteSpace(entity.PaymentMethodId)&& await  _payments.Exits(entity.PaymentMethodId)),
              !string.IsNullOrWhiteSpace(entity.AccountId)&& await _accounts.Exits(entity.AccountId),
                entity.Items!=null&&entity.Items.Count>0,
                entity.Status!=null&&entity.Status.Count>0,
                entity.ShippingAddress!=null,
            };
            foreach (var item in arr)
            {
                if (item == false)
                    return false;
            }
            return true;
        }
        async Task<int> Gettoltal(List<OrderItem> items)
        {
            int sum = 0;
            foreach (var item in items)
            {
                var product = await _products.GetbyId(item.ProductId!);
                if (product != null)
                    sum += (product.Price) * item.Quantity;
            }
            return sum;
        }

        async Task<int> DecreaseByVoucher(int total, string code)
        {
            var voucher = await _vouchers.GetVoucherbyCode(code);
            if (total < voucher.MinApply)
                return total;
            double result;
            if (voucher.IsValue)
                result = total - voucher.Value;
            else
            {
                var valuedef = total * (voucher.Value) / 100;
                if (valuedef > voucher.MaxReduce)//lớn hơn giảm tối đa thì lấy giảm tối đa
                    valuedef = voucher.MaxReduce;
                result = total - valuedef;
            }
            return (int)result;

        }


    }
}
