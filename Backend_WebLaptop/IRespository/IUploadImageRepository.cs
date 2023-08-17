using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IUploadImageRepository
    {
       Task<string> UploadProfile_Image(ImageUpload imageUpload,string filename);
       Task<List<string>> UploadProduct_Image(ImageUpload imageUpload, string filename);
    }
}
