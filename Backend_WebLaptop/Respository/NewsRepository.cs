using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using SharpCompress.Common;
using System.Linq;

namespace Backend_WebLaptop.Respository
{
    public class NewsRepository : INewRepository
    {
        private readonly IMongoCollection<News> _news;
        private readonly IUploadImageRepository _upload;

        public NewsRepository(IDatabaseService database,IUploadImageRepository upload)
        {
            _news = database.Get_News_Collections();
            _upload = upload;
        }
        public async Task<bool> DeletebyId(string id)
        {
            var rs = await _news.FindOneAndDeleteAsync(e => e.Id == id);
            if (rs.Images != null)
                foreach (var item in rs.Images)
                    await _upload.Delete_Image(3, item);
            return rs != null;
        }

        public async Task<PagingResult<News>> GetAll(string? keywords, DateTime? startdate, DateTime? enddate, int pageindex , int pagesize , string? sort = "date_desc")
        {
            //filter
            var result = string.IsNullOrWhiteSpace(keywords) ? await _news.FindSync(_ => true).ToListAsync() : await _news.FindSync(e => e.Title!.Contains(keywords)).ToListAsync();
            if (startdate != null)
                result = result.FindAll(e => e.CreateAt >= startdate);
            if (enddate != null)
                result = result.FindAll(e => e.CreateAt <= enddate);
            //sort
            result = sort switch
            {
                "date_desc" => result.OrderByDescending(e => e.CreateAt).ToList(),
                _ => result.OrderBy(e => e.CreateAt).ToList()
            };
            return new PagingResult<News>
            {
                Items = result.Skip((pageindex - 1) * pagesize).Take(pagesize),
                PageSize = pagesize,
                PageIndex = pageindex,
                TotalCount = result.Count
            };
        }

        public async Task<News> GetbyId(string id) => await _news.FindSync(e => e.Id == id).FirstOrDefaultAsync();

        public async Task<News> Insert(ImageUpload<News> entity)
        {
            //Validate
            if (entity.Data == null)
                throw new Exception("Data invalid");
            if (entity.Images == null || entity.Images.Count == 0)
                throw new Exception("Must have at least one image");
            entity.Data.Id = ObjectId.GenerateNewId(DateTime.Now).ToString();
            //Upload ảnh
            entity.Data.Images = await _upload.UploadPost_Image(entity);
            await _news.InsertOneAsync(entity.Data);
            return entity.Data;
           
        }

        public async Task<News> Update(ImageUpload<News> entity)
        {
            //Validate
            if (entity.Data == null)
                throw new Exception("Data invalid");
           var curent= await GetbyId(entity.Data!.Id!) ?? throw new Exception("This record does not exist");
            if (entity.Images != null && entity.Images.Count > 0)
            {
                //xoá ảnh cũ 
                if (entity.Data.Images != null)
                    foreach (var item in entity.Data.Images)
                        await _upload.Delete_Image(3, item);
                //Upload ảnh mới
                entity.Data.Images = await _upload.UploadPost_Image(entity);
            }
            else
                entity.Data.Images = curent.Images;
            entity.Data.CreateAt = curent.CreateAt;
           var result= await _news.FindOneAndReplaceAsync(e=>e.Id==entity.Data.Id, entity.Data);
            return result;
        }
        public async Task<long> GetTotalCreatebyTime(DateTime start, DateTime end)=> await _news.CountDocumentsAsync(e => e.CreateAt >= start && e.CreateAt <= end);


    }
}
