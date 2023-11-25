using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

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

        public async Task<Order> Checkout(Order entity)
        {
            throw new Exception("");
        }

        public async Task<PagingResult<Order>> GetAllOrders(string? accountid, int? status,
            bool? isPaid, string? paymentId, int? minPaid, int? maxPaid, DateTime? startdate, DateTime? enddate,
            string sort, int pagesize = 25, int pageindex = 1)
        {
            //Fillter
            var filter = Builders<Order>.Filter;
            var builderFilter = filter.Empty;
            var project = Builders<Order>.Projection.Slice("Status", -1);
            //&= là toán tử and
            if (accountid != null)
                builderFilter &= filter.Eq(e => e.AccountId, accountid);
            if (isPaid != null)
                builderFilter &= filter.Eq(e => e.IsPaid, isPaid);
            if(startdate!=null)
                builderFilter &= filter.Gte(e => e.CreateAt, startdate);
            if(enddate!=null)
                builderFilter &= filter.Gte(e => e.CreateAt, startdate);
            if (status != null)
                builderFilter &= filter.Eq(e => e.Status!.Code,status );
            var orders = await _orders.FindSync(builderFilter).ToListAsync();
            if (minPaid != null)
                orders = orders.FindAll(e => e.Total >= minPaid).ToList();
            if (maxPaid != null)
                orders = orders.FindAll(e => e.Total >= maxPaid).ToList();
            //Sort
            orders = sort switch
            {
                "date_desc" => orders.OrderByDescending(e => e.CreateAt).ToList(),
                "ispaid" => orders.OrderBy(e => e.IsPaid).ToList(),
                "ispaid_desc" =>    orders.OrderByDescending(e => e.IsPaid).ToList(),
                "total"=> orders.OrderBy(e => e.Total).ToList(),
                "total_desc" => orders.OrderByDescending(e => e.Total).ToList(),
                _ => orders.OrderBy(e=>e.CreateAt).ToList(),
               

            } ;
            return new PagingResult<Order>
            {
                Items = orders.Skip((pageindex - 1) * pagesize).Take(pagesize),
                PageIndex = pageindex,
                PageSize = pagesize,
                TotalCount = orders.Count
            };
        }


        public async Task<Order> CreateOrder(Order entity,bool isAdmin)
        {
            //thêm trạng thái đơn hàng
            var Order = await Validate(entity, isAdmin);
            {
                Order.Total = await Gettoltal(entity.Items);
                Order.CreateAt = DateTime.Now;
                //Có voucher
                Order.Paid = Order.Voucher != null ? await DecreaseByVoucher(Order.Items, Order.Voucher.Code!, Order.Total) : Order.Total;
                //giảm số lượng sản phẩm và tăng số lượng bán.
                //thêm vào database
                var Listtask = new List<Task> {
                    _products.DecreaseQuantity(Order.Items!),
                    _orders.InsertOneAsync(Order),
                };
                if (Order.Voucher != null && Order.Voucher.Code != null)
                    Listtask.Add(_vouchers.Decrease(Order.Voucher.Code));
                await Task.WhenAll(Listtask.ToArray());
            }
            return Order;

        }


        public async Task<bool> DeleteOrder(string id)
        {
            var rs = await _orders.DeleteOneAsync(e => e.Id == id);
            return rs.DeletedCount > 0;
        }

        public async Task<Order> EditOrder(Order entity, string id)
        {
            var curent = await _orders.FindSync(e => e.Id == id).FirstOrDefaultAsync() ?? throw new Exception("item does not exist");
            var filter = Builders<Order>.Filter.Eq(e => e.Id, id);
            var updatebuilder = new List<UpdateDefinition<Order>>();

            //tạo cac thay đổi
            if (entity.Status != null)
                updatebuilder.Add(Builders<Order>.Update.Set(e => e.Status, entity.Status));
            if (entity.Items != null)
                updatebuilder.Add(Builders<Order>.Update.Set(e => e.Items, entity.Items));
            if (entity.PaymentMethod != null)
                updatebuilder.Add(Builders<Order>.Update.Set(e => e.PaymentMethod, entity.PaymentMethod));
            if (entity.ShippingAddress != null)
                updatebuilder.Add(Builders<Order>.Update.Set(e => e.ShippingAddress, entity.ShippingAddress));
            if (entity.Paid != curent.Paid)
                updatebuilder.Add(Builders<Order>.Update.Set(e => e.Paid, entity.Paid));
            var update = Builders<Order>.Update.Combine(updatebuilder);
            return await _orders.FindOneAndUpdateAsync(filter, update);
        }

        public Task<Order> UpdateStatus(string id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Validate Data
        /// Type order:Default is customer Equal true is admin Create Order
        /// </summary>
        /// <param name="entity"> Order</param>
        /// <param name="type"> </param>
        /// <returns></returns>
        async Task<Order> Validate(Order entity,bool byAdmin)
        {
            if (entity.ShippingAddress == null)
                throw new Exception("Người nhận không hợp lệ");
            //Nếu là khách đặt (mặc định)
            if (!byAdmin )
            {
                entity.Status = new Shipping(1);
                //kiểm tra địa chỉ giao hàng
                if (entity.ShippingAddress == null || (entity.ShippingAddress.Id != null && await _shippingaddress.GetbyId(entity.ShippingAddress.Id) == null))
                    throw new Exception("Địa chỉ giao hàng không hợp lệ");
                entity.ShippingAddress = await _shippingaddress.GetbyId(entity.ShippingAddress.Id!);
            }
            //Admin tạo đơn hàng
            //kiểm tra sản phẩm
            if (entity.Items.Count == 0)
                throw new Exception("Đơn hàng phải có ít nhất 1 sản phẩm");
            if (entity.Items.Any(e => e.Product == null))
                throw new Exception("Yêu cầu không hợp lệ");
            //kiểm tra phương thức thanh toán
            if (entity.PaymentMethod != null )
            {
                var payment = await _payments.GetbyId(entity.PaymentMethod.Id!) ?? throw new Exception("Phương thức thanh toán không hợp lệ");
                entity.PaymentMethod = payment;
            }

            //Kiểm tra voucher
            if (entity.Voucher != null)
            {
                var voucher =await  _vouchers.GetVoucherbyCode(entity.Voucher.Code!) ?? throw new Exception("Mã giảm giá không hợp lệ");
                if (voucher.Quantity <= 0)
                    throw new Exception("Mã giảm giá đã hết số lượt sử dụng");
                entity.Voucher = voucher;
            }
            //kiểm tra số lượng sản phẩm có đủ để bán hay không
             entity.Items= await _products.Cansell(entity.Items);
            return entity;
        }
        async Task<int> Gettoltal(List<OrderItem> items)
        {
            int sum = 0;
            foreach (var item in items)
            {
                var product = await _products.GetbyId(item.Product!.Id!);
                if (product != null)
                    sum += (product.Price) * item.Quantity;
            }
            return sum;
        }

        async Task<int> DecreaseByVoucher(List<OrderItem> OrderItems, string code,int Total)
        {
            var voucher = await _vouchers.GetVoucherbyCode(code);
            //kiểm tra voucher còn sử dụng được không
            if (!_vouchers.IsValidCode(voucher))
                throw new Exception("Voucher không thể sử dụng");
            var total_for_decrease = 0;//tổng giá trị của sản phẩm được giảm giá
            //trường hợp voucher giới hạn sản phẩm
            if (voucher.Products != null && voucher.Products.Count >= 0)
                foreach (var item in OrderItems)
                {
                    if (voucher.Products.Contains(item.Product!.Id!))
                        total_for_decrease += item.Product.Price;
                }
            else
                total_for_decrease = Total;
            var remaining_no_decrease = Total - total_for_decrease;//số tiền còn lại 
            //kiểm tra đã đạt đơn tối thiểu để sử dụng voucher chưa
            if (voucher.MinApply != null && total_for_decrease < voucher.MinApply)
                if (total_for_decrease < voucher.MinApply)
                    throw new Exception("Đơn hàng chưa đủ điều kiện sử dụng Voucher");
            double result;
            if (voucher.IsValue)//trường hợp giảm trực tiếp
                result = (total_for_decrease - voucher.Value)+remaining_no_decrease;
            else //giảm theo phần trăm
            {
                var valuedef = total_for_decrease * (voucher.Value) / 100;
                if (valuedef > voucher.MaxReduce)//lớn hơn giảm tối đa thì lấy giảm tối đa
                    valuedef = voucher.MaxReduce;
                result = Total - valuedef;
            }
            return (int)result;

        }

        public Task<Order> CheckOrder(Order entity)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetOrderbyId(string id)
        {
            throw new NotImplementedException();
        }

        public Task<long> Get_toltalSell(DateTime? start, DateTime? end)
        {
            throw new NotImplementedException();
        }

       
    }
}
