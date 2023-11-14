using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Driver;

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

        public async Task<PagingResult<Comment>> GetAll(int pageindex, string productId, int size)
        {

            var comments = await _comments.FindAsync(e => e.ProductId == productId);
            var result = new PagingResult<Comment>
            {
                PageIndex = pageindex,
                PageSize = size,
                Items = comments.ToEnumerable<Comment>().OrderBy(e => e.CreateAt).Skip((pageindex - 1) * size).Take(size)
            };
            return result;
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
