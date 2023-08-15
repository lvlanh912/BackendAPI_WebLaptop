using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IMongoCollection<Comment> _Comments;
        private readonly IAccountResposytory _Accounts;
        private readonly IProductRepository _Products;
        public CommentRepository(IDatabase_Service database_Service, IAccountResposytory Accounts,IProductRepository Products)
        {
            _Comments=database_Service.Get_Comments_Collection();
            _Accounts = Accounts;
            _Products=Products;
        }
        public async Task<bool> DeletebyId(string id)
        {
            var rs = await _Comments.DeleteOneAsync(e => e.Id == id);
            return rs.DeletedCount != 0;
        }

        public async Task<PagingResult<Comment>> GetAll(int pageindex, string ProductId, int size)
        {

            var comments =await _Comments.FindAsync(e => e.ProductId == ProductId);
            var result = new PagingResult<Comment>{ 
                PageIndex=pageindex,
                PageSize=size,
                Items= comments.ToEnumerable<Comment>().OrderBy(e => e.CreateAt).Skip((pageindex - 1) * size).Take(size)
            };
            return result;
        }
        public async Task<Comment> GetbyId(string id)=> await _Comments.FindSync(e => e.Id == id).FirstOrDefaultAsync();

        public async Task<bool> Insert(Comment entity)
        {
            if( await ValidateData(entity))
            {
                entity.CreateAt=DateTime.Now;
                await _Comments.InsertOneAsync(entity);
                return true;
            }
            return false;
        }
         async Task<bool> ValidateData(Comment entity)
        {
            List<bool> result = new()
            {
               entity.AccountID!=null&& await _Accounts.Exits(entity.AccountID),
               entity.Star>1&&entity.Star<=5,
               entity.ProductId!=null&& await _Products.Exits(entity.ProductId),
            };
            foreach(var item in result)
                if (item == false)
                    return item;
            return true;
        }
    }
}
