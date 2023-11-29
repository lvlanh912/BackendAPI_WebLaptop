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
        private readonly IProductRepository _products;
        public CommentRepository(IDatabaseService databaseService, IAccountRepository accounts, IProductRepository products)
        {
            _comments = databaseService.Get_Comments_Collection();
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

        public async Task<bool> Insert(Comment entity)
        {
            if (await ValidateData(entity))
            {
                entity.CreateAt = DateTime.Now;
                await _comments.InsertOneAsync(entity);
                return true;
            }
            return false;
        }
        public async Task<bool> Update(Comment entity)
        {
            if (await ValidateData(entity))
            {
                entity.CreateAt = DateTime.Now;
                await _comments.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
                return true;
            }
            return false;
        }
        async Task<bool> ValidateData(Comment entity)
        {
            List<bool> result = new()
            {
               entity.AccountId!=null&& await _accounts.Exits(entity.AccountId),
               entity.Star>1&&entity.Star<=5,
               entity.ProductId!=null&& await _products.Exits(entity.ProductId),
            };
            foreach (var item in result)
                if (item == false)
                    return item;
            return true;
        }
    }
}
