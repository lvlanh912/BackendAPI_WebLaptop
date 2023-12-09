using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;

namespace Backend_WebLaptop.Respository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IMongoCollection<Comment> _comments;
        private readonly IAccountRepository _accounts;
        private readonly IOrderRepository _orders;
        private readonly IProductRepository _products;
        public CommentRepository(IDatabaseService databaseService, IAccountRepository accounts, IOrderRepository orders,IProductRepository products)
        {
            _comments = databaseService.Get_Comments_Collection();
            _orders = orders;
            _accounts = accounts;
            _products = products;
        }
        public async Task<bool> DeletebyId(string id)
        {
            var rs = await _comments.DeleteOneAsync(e => e.Id == id);
            return rs.DeletedCount != 0;
        }

       public async Task<PagingResult<Comment>> GetAll(string? accountid, string? productId, string? keywords, string sort, int pageindex, int pagesize)
        {
            //Filter
            var filter = Builders<Comment>.Filter;
            var builderFilter = filter.Empty;
            if (accountid != null)
                builderFilter &= filter.Eq(e => e.AccountId, accountid);
            if(productId!=null)
                builderFilter &= filter.Eq(e => e.ProductId, productId);
            if(keywords!=null)
                builderFilter &= filter.Regex(e => e.Conntent, new BsonRegularExpression($".*{keywords}"));
            var result = await _comments.FindSync(builderFilter).ToListAsync();
            //sort
            result = sort switch
            {
                "date_desc" => result.OrderByDescending(e => e.CreateAt).ToList(),
                "star"=> result.OrderBy(e => e.Star).ToList(),
                "star_desc" => result.OrderByDescending(e => e.Star).ToList(),
                _ => result.OrderBy(e => e.CreateAt).ToList(),
                
            };
           
            return new PagingResult<Comment>
            {
                PageIndex = pageindex,
                PageSize = pagesize,
                Items = result.Skip((pageindex - 1) * pagesize).Take(pagesize),
                TotalCount=result.Count
                
            };
    
        }
        public async Task<Comment> GetbyId(string id) => await _comments.FindSync(e => e.Id == id).FirstOrDefaultAsync();

        public async Task<long> GetTotalCreatebyTime(DateTime start, DateTime end)=> await _comments.CountDocumentsAsync(e => e.CreateAt >= start && e.CreateAt <= end);


        public async Task<Comment> Insert(Comment entity)
        {
            await ValidateData(entity);
            //kiểm tra đã mua sản phẩm này chưa
            if(!await _orders.IsBought(entity.ProductId!, entity.AccountId!))
                throw new Exception("Bạn không thể đánh giá sản phẩm này do chưa hoàn thành mua hàng");
            //kiểm tra đã có đánh giá trước đó chưa
            var comment = await _comments.FindSync(e => e.ProductId == entity.ProductId && e.AccountId == entity.AccountId).FirstOrDefaultAsync();
            if (comment is not null)
                throw new Exception("Bạn đã đánh giá sản phẩm này trước đó rồi");
            await _comments.InsertOneAsync(entity);
            return entity;
        }
        public async Task<bool> Update(Comment entity)
        {
            await ValidateData(entity);
            await _comments.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
            return true;
        }
        async Task ValidateData(Comment entity)
        {
            if (entity.Star < 1 || entity.Star > 5)
                throw new Exception("Chất lượng không hợp lệ");
            if (entity.ProductId is null)
                throw new Exception("Không tồn tại sản phẩm");
            if(!await _products.Exits(entity.ProductId))
                throw new Exception("Sản phẩm không hợp lệ");
        }
    }
}
