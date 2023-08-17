﻿using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class AccountRepository : IAccountResposytory
    {
        private readonly IMongoCollection<Account>? Accounts;
        private IUploadImageRepository _Upload;

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

        public async Task<Account> GetbyId(string id)
            => await Accounts.Find(e => e.Id == id).FirstOrDefaultAsync();


        public async Task<Account> Insert(Account e, ImageUpload? imageUpload)
        {
            if (string.IsNullOrWhiteSpace(e.Username)|| string.IsNullOrWhiteSpace(e.Password) ||
                string.IsNullOrWhiteSpace(e.Email))
                throw new Exception("Data invalid");

            if(await ExitsByUserName(e.Username))
                throw new Exception("username has been taken");

            e.Id = ObjectId.GenerateNewId(DateTime.Now).ToString();
            e.WardID = new ObjectId(e.WardID).ToString();
            e.Profile_image = await _Upload.UploadProfile_Image(imageUpload!, e.Id);
            await Accounts!.InsertOneAsync(e);
            
            return e;
        }
        public async Task<Account> Update(Account entity, ImageUpload? imageUpload)
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
            if (imageUpload != null)
            {
                entity.Profile_image =await _Upload.UploadProfile_Image(imageUpload, curent.Id!);
            }
            else
                entity.Profile_image = curent.Profile_image;
            await Accounts.FindOneAndReplaceAsync(x => x.Id == entity.Id, entity);
            return entity;
        }

    }
}
