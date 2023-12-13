using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Backend_WebLaptop.Respository
{
    public class StatisticRepository:IStatisticRepository
    {
        private readonly IMongoCollection<Product> _product;
        private readonly IMongoCollection<Order> _order;


        public StatisticRepository(IDatabaseService databaseService)
        {
            _product = databaseService.Get_Products_Collection();
            _order = databaseService.Get_Orders_Collection();

        }

        public async Task<List<long>> GetOrderPerMonth(DateTime startTime, DateTime endTime)
        {
            if (endTime <= startTime)
                throw new Exception("Invalid date");
            var monthCount = Getmonth(startTime, endTime);
            if (monthCount > 12)
                throw new Exception("Giới hạn trong khoảng 12 tháng");
            var task = new List<Task<long>>();
            //ngày đầu tiên và cuối cùng trong tháng
            DateTime FistDayofMonth= new(startTime.Year, startTime.Month, 1);
            DateTime EndDayofMonth;
            for (int i= 0; i < monthCount; i++)
            {
                EndDayofMonth = new DateTime(FistDayofMonth.Year, FistDayofMonth.Month, DateTime.DaysInMonth(FistDayofMonth.Year, FistDayofMonth.Month)).AddDays(1).AddTicks(-1);
                var filter = Builders<Order>.Filter.And(
                    Builders<Order>.Filter.Gte(e => e.CreateAt, FistDayofMonth),
                    Builders<Order>.Filter.Lte(e => e.CreateAt, EndDayofMonth)
                    );
                task.Add(_order.CountDocumentsAsync(filter));
                FistDayofMonth = FistDayofMonth.AddMonths(1);
            }
            var result = new List<long>();
            await Task.WhenAll(task);
            foreach (var item in task)
            {
                result.Add(item.Result);
            }
            return result;



        }
        public int Getmonth(DateTime startTime, DateTime endTime)
        {
            var result = endTime.Month - startTime.Month +1;
            if (endTime.Year > startTime.Year)//trường hợp sang năm
                result = endTime.Month + (12*(endTime.Year-startTime.Year) - startTime.Month) +1;
            return result;
        }

        public async Task<object> GetOrderStatistic()
        {

            var _waitConfirm = _order.CountDocumentsAsync(e => e.Status!.Code == 1);

            var _shipping = _order.CountDocumentsAsync(e => e.Status!.Code == 2);

            var _success = _order.CountDocumentsAsync(e=>e.Status!.Code==3);
            var _canceled = _order.CountDocumentsAsync(e => e.Status!.Code == 0);
            await Task.WhenAll(_waitConfirm, _shipping, _success, _canceled);
            return new
            {
                waitConfirm = await _waitConfirm,
                shipping=await _shipping,
                success=await _success,
                canceled=await _canceled,
            };

        }

        public async Task<List<long>> GetProductAddStatistic(int year)
        {
            var result = new List<long>();
            var task = new List<Task<long>>();
            for (int i= 1; i <= 12; i++)
                task.Add(_product.CountDocumentsAsync(e => e.CreateAt.Month == i&&e.CreateAt.Year==year));
            //chờ tác vụ hoàn thành
            await Task.WhenAll(task);
            foreach (var item in task)
            {
                result.Add(item.Result);
            }
            return result;
        }

        public async Task<object> GetStockStatistic()
        {
            var _haveStock =_product.CountDocumentsAsync(e=>e.Stock>0);
            var _outOfStock= _product.CountDocumentsAsync(e => e.Stock <= 0);
            await Task.WhenAll(_haveStock, _outOfStock);
            return new 
            {
                haveStock = await _haveStock,
                outOfStock=await _outOfStock
            };

        }

        public async Task<List<Product>> GetTop(int total, DateTime starTime, DateTime? endTime,int type)
        {
            var filter = Builders<Product>.Filter.Empty;
            filter &= Builders<Product>.Filter.Gte(e => e.CreateAt, starTime);
            if(endTime is not null)
                filter &= Builders<Product>.Filter.Lte(e => e.CreateAt, endTime);
            var result = type switch
            {
                1=> await _product.Find(filter).SortByDescending(e => e.Sold).Limit(total).ToListAsync(),
                2=> await _product.Find(filter).SortByDescending(e => e.View).Limit(total).ToListAsync(),
                3 => await _product.Find(filter).SortByDescending(e => e.Stock).Limit(total).ToListAsync(),
                _ => throw new Exception("Invalid type")
            };
            return result;
         
        }

        public async Task<object> GetRevenue(DateTime startTime, DateTime endTime)
        {
            var filter = Builders<Order>.Filter.And(
                 Builders<Order>.Filter.Gte(e => e.Status!.Code, 3),
                 Builders<Order>.Filter.Gte(e => e.CreateAt, startTime),
                  Builders<Order>.Filter.Lte(e => e.CreateAt, endTime)
                );
            var totalValue = 0;
            var totalIncome = 0;
            var list =await _order.FindSync(filter).ToListAsync();
            list.ForEach(item =>
            {
                totalValue += item.Total;
                totalIncome += item.Paid;
            });

            return new
            {
                startDate = startTime,
                endDate=endTime,
                totalvalue = totalValue,
                totalPaid = totalIncome
            };
        }
    }
}
