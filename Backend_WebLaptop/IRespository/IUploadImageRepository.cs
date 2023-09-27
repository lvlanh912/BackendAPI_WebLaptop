using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IUploadImageRepository
    {
        Task<string> UploadProfile_Image(ImageUpload<Account> entity);
        Task<List<string>> UploadProduct_Image(ImageUpload<Product> entity);
    }
}
