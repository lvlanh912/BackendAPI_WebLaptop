using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IUploadImageRepository
    {
        Task<string> UploadProfile_Image(ImageUpload<Account> entity);
        Task<List<string>> UploadProduct_Image(ImageUpload<Product> entity);
        Task<List<string>> UploadPost_Image(ImageUpload<News> entity);

        /// <summary>
        /// Delete file in local.
        /// type: 1 is delete avatar
        /// type: 2 is delete product image
        /// type: 3 is delete post image
        /// </summary>
        Task Delete_Image(int type, string namefile);
    }
}
