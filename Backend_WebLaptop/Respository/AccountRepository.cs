using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IMongoCollection<Account>? Accounts;
        private readonly IUploadImageRepository _Upload;

        public AccountRepository(IDatabase_Service database_Service, IUploadImageRepository Upload)
        {
            Accounts = database_Service.Get_Accounts_Collection();
            _Upload = Upload;
        }
        public async Task<bool> DeletebyId(string id)
        {
           var rs= await Accounts.DeleteOneAsync(e => e.Id == id);
            return rs.DeletedCount!=0;
        }

        public async Task<bool> Exits(string id)
        {
            var rs = await Accounts.FindSync(e => e.Id == id).FirstOrDefaultAsync();
            return rs != null;
        }
        public async Task<bool> ExitsByUserName(string username)
        {
            var rs = await Accounts.FindSync(e => e.Username == username).FirstOrDefaultAsync();
            return rs != null;
        }

        public async Task<PagingResult<Account>> GetAll(string? keywords, int pageindex, int pagesize)
        {
            var result = new PagingResult<Account>();
            List<Account> accounts;
            //filter
            if (keywords == null)
            {
                accounts = await Accounts.Find(_ => true).ToListAsync();
            }
            else
                accounts = await Accounts.Find(filter: e => e.Username!.Contains(keywords) || e.Fullname!.Trim().Contains(keywords)).ToListAsync();
            result.TotalCount = accounts.Count;
            result.Items = accounts.Skip((pageindex - 1) * pagesize);
            result.Items = result.Items.Take(pagesize);
            result.PageIndex = pageindex;
            result.PageSize = pagesize;
            //var a = await Users.Find(_ => true).ToListAsync();
            return result;
        }
        public async Task<Account> GetbyId(string id)=> await Accounts.Find(e => e.Id == id).FirstOrDefaultAsync();

        public async Task<Account> Insert(ImageUpload<Account> entity)
        {
            if (string.IsNullOrWhiteSpace(entity.data!.Username)|| string.IsNullOrWhiteSpace(entity.data.Password) ||
                string.IsNullOrWhiteSpace(entity.data.Email))
                throw new Exception("Data invalid");
            if(await ExitsByUserName(entity.data.Username))
                throw new Exception("username has been taken");
            entity.data.Id = ObjectId.GenerateNewId(DateTime.Now).ToString();
            entity.data.WardID = new ObjectId(entity.data.WardID).ToString();
            if(entity.images != null)
            entity.data.Profile_image = await _Upload.UploadProfile_Image(entity);
            await Accounts!.InsertOneAsync(entity.data);
            //khởi tạo giỏ hàng
            return entity.data;
        }
        public async Task<Account> Update(ImageUpload<Account> entity)
        {
            var curent = await GetbyId(entity.data!.Id!);
            if (curent == null)
            {
                throw new Exception("This record does not exits");
            }
            entity.data.Username = curent.Username;//cannot update username
            entity.data.Password ??= curent.Password;
            entity.data.Email ??= curent.Email;
            entity.data.Address ??= curent.Address;
            entity.data.Roles ??= curent.Roles;
            entity.data.CreateAt = curent.CreateAt;
            entity.data.WardID ??= curent.WardID;
            entity.data.Sex ??= curent.Sex;
            entity.data.Phone ??= curent.Phone;
            if (entity.images != null)
            {
                entity.data.Profile_image =await _Upload.UploadProfile_Image(entity);
            }
            else
                entity.data.Profile_image = curent.Profile_image;
            await Accounts.FindOneAndReplaceAsync(x => x.Id == entity.data.Id, entity.data);
            return entity.data;
        }

    }
}
