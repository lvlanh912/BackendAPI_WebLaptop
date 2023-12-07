using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Xml.Linq;

namespace Backend_WebLaptop.Respository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IMongoCollection<Account>? _accounts;
        private readonly IUploadImageRepository _upload;
        private readonly ICartRepository _carts;
        private readonly IAddressRepository _address;
        private readonly IMongoCollection<Order>? _order;
        private readonly IMongoCollection<Comment>? _comment;

        public AccountRepository(IDatabaseService databaseService, IUploadImageRepository upload, ICartRepository carts, IAddressRepository address)
        {
            _accounts = databaseService.Get_Accounts_Collection();
            _order = databaseService.Get_Orders_Collection();
            _comment = databaseService.Get_Comments_Collection();
            _upload = upload;
            _carts = carts;
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
                    "fullname"=> await _accounts.Find(filter: e => e.Fullname!.Contains(keywords)).ToListAsync(),
                    "email"=> await _accounts.Find(filter: e => e.Email!.Contains(keywords)).ToListAsync(),
                    "address"=> await _accounts.Find(filter: e => e.Address!.Contains(keywords)).ToListAsync(),
                    "phone"=> await _accounts.Find(filter: e => e.Phone!.Equals(keywords)).ToListAsync(),
                    _ => await _accounts.Find(filter: e => e.Username!.Contains(keywords) || e.Fullname!.Trim().Contains(keywords)).ToListAsync(),
                };
            }
            else
                accounts = await _accounts.Find(_ => true).ToListAsync();
            if (startdate != null)
                accounts = accounts.FindAll(e => e.CreateAt >= startdate).ToList();
            if (enddate != null)
                accounts = accounts.FindAll(e => e.CreateAt <= enddate).ToList();
            if (role != null)
                accounts = accounts.FindAll(e => e.Role == role).ToList();
            if (gender != null)
                accounts = accounts.FindAll(e => e.Sex == gender).ToList();

            //sort
            result.Items = sort switch
            {
                "date_desc" => accounts.OrderByDescending(e => e.CreateAt),
                "username" => accounts.OrderBy(e => e.Username),
                "username_desc" => accounts.OrderByDescending(e => e.Username),
                _ => accounts.OrderBy(e => e.CreateAt),
            };
            //pagging
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
            var rs = await _accounts.FindOneAndDeleteAsync(e => e.Id == id);
            if (rs != null&&rs.ProfileImage!=null)
                await _upload.Delete_Image(1, rs.ProfileImage);
            await _carts.DeleteCart(id);
            return rs != null;
        }

        public async Task<bool> Exits(string id)
        {
            var rs = await _accounts.FindSync(e => e.Id == id).FirstOrDefaultAsync();
            return rs != null;
        }
        public async Task<bool> ExitsByUserName(string username)
        {
            var rs = await _accounts.FindSync(e => e.Username == username).FirstOrDefaultAsync();
            return rs != null;
        }

        async Task ValidateAccount(Account entity)
        {
            if (entity.Phone != null)
            {
                var rs = await _accounts.FindSync(e => e.Phone == entity.Phone).FirstOrDefaultAsync();
                    if(rs!=null) throw new Exception("Số điện thoại đã được sử dụng");
            }
            if (string.IsNullOrEmpty(entity.Username)) throw new Exception("Tên đăng nhập Không được để trống");
            if (string.IsNullOrEmpty(entity.Password)) throw new Exception("Mật khẩu không được để trống");
            var V_username = await _accounts.FindSync(e => e.Username == entity.Username).FirstOrDefaultAsync();
            if(V_username!=null)  throw new Exception("Tên người dùng đã được sử dụng");

            if (string.IsNullOrEmpty(entity.Email)) throw new Exception("Email Không được để trống");
            var V_email = await _accounts.FindSync(e => e.Email == entity.Email).FirstOrDefaultAsync();
            if(V_email!=null) throw new Exception("Địa chỉ email đã được sử dụng");
        }

        public async Task<Account> GetbyId(string id) => await _accounts.Find(e => e.Id == id).FirstOrDefaultAsync();

        public async Task<Account> Insert(ImageUpload<Account> entity)
        {
            await  ValidateAccount(entity.Data!);
            entity.Data!.Id = ObjectId.GenerateNewId(DateTime.Now).ToString();
            if (entity.Images != null && entity.Images.Count > 0)
                entity.Data.ProfileImage = await _upload.UploadProfile_Image(entity);
            if(entity.Data.Address!=null)
                entity.Data.Address += '-' + await _address.GetAddress(entity.Data.WardId!);
            await _accounts!.InsertOneAsync(entity.Data);
            //khởi tạo giỏ hàng
            await _carts.Create(entity.Data.Id);
            return entity.Data;
        }
        public async Task<Account> Update(ImageUpload<Account> entity)
        {
            var curent = await GetbyId(entity.Data!.Id!) ?? throw new Exception("This record does not exist");
            if (Convert.ToInt32(entity.Data.Phone) > 999999999)
                throw new Exception("Số điện thoại phải 10 chữ số");
            entity.Data.Username = curent.Username;//cannot update username
            entity.Data.ProfileImage ??= curent.ProfileImage;
            entity.Data.Password ??= curent.Password;
            entity.Data.Fullname ??= curent.Fullname;
            entity.Data.Email ??= curent.Email;
            entity.Data.Address ??= curent.Address;
            entity.Data.Role ??= curent.Role;
            entity.Data.CreateAt = curent.CreateAt;
            entity.Data.WardId ??= curent.WardId;
            entity.Data.Sex ??= curent.Sex;
            entity.Data.Phone ??= curent.Phone;
            if (entity.Images!.Count != 0)
            {
                entity.Data.ProfileImage = await _upload.UploadProfile_Image(entity);
            }
            else
                entity.Data.ProfileImage = curent.ProfileImage;
            await _accounts.FindOneAndReplaceAsync(x => x.Id == entity.Data.Id, entity.Data);
            return entity.Data;
        }

        public async Task<int> GetTotalOrder(string accountId)
        {
            var filter = Builders<Order>.Filter.Eq(e => e.AccountId, accountId);
            var rs = await _order.FindSync(filter).ToListAsync();
            return rs.Count;

        }

        public async Task<int> GetTotalComment(string accountId)
        {
            var filter = Builders<Comment>.Filter.Eq(e => e.AccountId, accountId);
            var rs = await _comment.FindSync(filter).ToListAsync();
            return rs.Count;
        }

        public async Task<long> GetTotalCreatebyTime(DateTime start, DateTime end)
        {

            return await _accounts.CountDocumentsAsync(e => e.CreateAt >= start && e.CreateAt <= end);
        }

        public async Task UpdateImage(ImageUpload<Account> entity)
        {
            if (entity.Images!.Count != 0)
            {
                var update = Builders<Account>.Update.Set(e => e.ProfileImage, await _upload.UploadProfile_Image(entity));
                 await _accounts.UpdateOneAsync(e => e.Id == entity.Data!.Id, update);
            }
        }

        public async Task Updateinfor(Account entity)
        {
            var task = new List<Task>();
            //update họ tên
            if (!string.IsNullOrEmpty(entity.Fullname))
            {
                var update = Builders<Account>.Update.Set(e => e.Fullname, entity.Fullname);
                task.Add(_accounts.UpdateOneAsync(e => e.Id == entity.Id, update));
            }
            if (!string.IsNullOrEmpty(entity.Address))
            {
                var update = Builders<Account>.Update.Set(e => e.Address, entity.Address);
                task.Add(_accounts.UpdateOneAsync(e => e.Id == entity.Id, update));
            }
            await Task.WhenAll(task);
        }
    }
}
