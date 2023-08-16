using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class AccountRepository : IAccountResposytory
    {
        private readonly IMongoCollection<Account>? Accounts;

        public AccountRepository(IDatabase_Service database_Service) 
        {
            Accounts = database_Service.Get_Accounts_Collection();
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
                accounts = await Accounts.Find(filter: e => e.Username!.Contains(keywords) || e.Fullname.Contains(keywords)).ToListAsync();
            result.TotalCount = accounts.Count;
            result.Items = accounts.Skip((pageindex - 1) * pagesize);
            result.Items = result.Items.Take(pagesize);
            result.PageIndex = pageindex;
            result.PageSize = pagesize;
            //var a = await Users.Find(_ => true).ToListAsync();
            return result;
        }

        public async Task<Account> GetbyId(string id)
            => await Accounts.Find(e => e.Id == id).FirstOrDefaultAsync();


        public async Task<Account> Insert(Account e, IFormFile? avarta)
        {
            if (string.IsNullOrWhiteSpace(e.Username)|| string.IsNullOrWhiteSpace(e.Password) ||
                string.IsNullOrWhiteSpace(e.Email))
                throw new Exception("Data invalid");

            if(await ExitsByUserName(e.Username))
                throw new Exception("username has been taken");

            e.Id = ObjectId.GenerateNewId(DateTime.Now).ToString();
            e.WardID = new ObjectId(e.WardID).ToString();
            e.Profile_image =avarta!=null?e.Id+ Path.GetExtension(avarta.FileName):"";
            await Accounts!.InsertOneAsync(e);
            await Upload_ImageAsync(avarta, e.Id!);
            return e;
        }

        public async Task<Account> Update(Account entity)
        {
            var curent = await GetbyId(entity.Id!);
            if (curent == null)
            {
                throw new Exception("This record does not exits");
            }
            entity.Username = curent.Username;//cannot update username

            entity.Password ??= curent.Password;
            entity.Email ??= curent.Email;
            entity.Address ??= curent.Address;
            entity.Roles ??= curent.Roles;
            entity.CreateAt = curent.CreateAt;
            entity.WardID ??= curent.WardID;
            entity.Sex ??= curent.Sex;
            entity.Phone ??= curent.Phone;
            entity.Profile_image ??= curent.Profile_image;

            await Accounts.FindOneAndReplaceAsync(x => x.Id == entity.Id, entity);
            return entity;
        }

        async Task<string> Upload_ImageAsync(IFormFile? avarta,string name)
        {
            if (avarta==null||!avarta.ContentType.StartsWith("image/"))
                throw new Exception("This is not Images");
            string filename = name + Path.GetExtension(avarta.FileName);
            string path = Path.Combine(Directory.GetCurrentDirectory(),"Upload\\Image\\Avatar",filename);
            //lưu file
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await avarta.CopyToAsync(stream);
            }
            return "done";
        }
    }
}
