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
        private readonly ICartRepository _Carts;
        private readonly IAddressRepository _address;

        public AccountRepository(IDatabase_Service database_Service, IUploadImageRepository Upload, ICartRepository carts, IAddressRepository address)
        {
            Accounts = database_Service.Get_Accounts_Collection();
            _Upload = Upload;
            _Carts = carts;
            _address = address;
        }
        public async Task<PagingResult<Account>> GetAll(string? keywords, string? type, DateTime? startdate, DateTime? enddate, int? role, bool? gender, int pageindex, int pagesize, string sort)
        {
            var result = new PagingResult<Account>();
            List<Account> accounts;
            //filter
            if (keywords != null&&type!=null)
            {
                accounts = type switch
                {
                    "fullname"=> await Accounts.Find(filter: e => e.Fullname!.Contains(keywords)).ToListAsync(),
                    "email"=> await Accounts.Find(filter: e => e.Email!.Contains(keywords)).ToListAsync(),
                    "address"=> await Accounts.Find(filter: e => e.Address!.Contains(keywords)).ToListAsync(),
                    "phone"=> await Accounts.Find(filter: e => e.Phone!.Equals(keywords)).ToListAsync(),
                    _ => await Accounts.Find(filter: e => e.Username!.Contains(keywords) || e.Fullname!.Trim().Contains(keywords)).ToListAsync(),
                };
            }
            else
                accounts = await Accounts.Find(_ => true).ToListAsync();
            if (startdate != null)
                accounts = accounts.Where(e => e.CreateAt >= startdate).ToList();
            if (enddate != null)
                accounts = accounts.Where(e => e.CreateAt <= enddate).ToList();
            if (role != null)
                accounts = accounts.Where(e => e.Roles == role).ToList();
            if (gender != null)
                accounts = accounts.Where(e => e.Sex == gender).ToList();

            //sort
            result.Items = sort switch
            {
                "date_desc" => accounts.OrderByDescending(e => e.CreateAt),
                "username" => accounts.OrderBy(e => e.Username),
                "username_desc" => accounts.OrderByDescending(e => e.Username),
                _ => accounts.OrderBy(e => e.CreateAt),
            };
            result.TotalCount = accounts.Count;
            result.Items = result.Items.Skip((pageindex - 1) * pagesize);
            result.Items = result.Items.Take(pagesize);
            result.PageIndex = pageindex;
            result.PageSize = pagesize;
            //var a = await Users.Find(_ => true).ToListAsync();
            return result;
        }
        public async Task<bool> DeletebyId(string id)
        {
            var rs = await Accounts.DeleteOneAsync(e => e.Id == id);
            return rs.DeletedCount != 0;
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


        public async Task<Account> GetbyId(string id) => await Accounts.Find(e => e.Id == id).FirstOrDefaultAsync();

        public async Task<Account> Insert(ImageUpload<Account> entity)
        {
            if (string.IsNullOrWhiteSpace(entity.data!.Username) || string.IsNullOrWhiteSpace(entity.data.Password) ||
                string.IsNullOrWhiteSpace(entity.data.Email))
                throw new Exception("Data invalid");
            if (await ExitsByUserName(entity.data.Username))
                throw new Exception("username has been taken");
            entity.data.Id = ObjectId.GenerateNewId(DateTime.Now).ToString();
            if (entity.images != null && entity.images.Count > 0)
                entity.data.Profile_image = await _Upload.UploadProfile_Image(entity);
            entity.data.Address += '-' + await _address.GetAddress(entity.data.WardID!);
            await Accounts!.InsertOneAsync(entity.data);
            //khởi tạo giỏ hàng
            await _Carts.Create(entity.data.Id);
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
            entity.data.Fullname ??= curent.Fullname;
            entity.data.Email ??= curent.Email;
            entity.data.Address ??= curent.Address;
            entity.data.Roles ??= curent.Roles;
            entity.data.CreateAt = curent.CreateAt;
            entity.data.WardID ??= curent.WardID;
            entity.data.Sex ??= curent.Sex;
            entity.data.Phone ??= curent.Phone;
            if (entity.images!.Count != 0)
            {
                entity.data.Profile_image = await _Upload.UploadProfile_Image(entity);
            }
            else
                entity.data.Profile_image = curent.Profile_image;
            await Accounts.FindOneAndReplaceAsync(x => x.Id == entity.data.Id, entity.data);
            return entity.data;
        }

    }
}
