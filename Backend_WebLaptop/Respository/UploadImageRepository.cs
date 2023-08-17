using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.Respository
{
    public class UploadImageRepository : IUploadImageRepository
    {

        public async Task<List<string>> UploadProduct_Image(ImageUpload<Product> entity)//multi images
        {
            var list_name = new List<string>();
            string path = "products";
            if (entity.images == null)
                throw new Exception("Cannot upload null image");
            else
            {
                for (int i = 0; i < entity.images.Count; i++)
                {
                    if (!IsImage(entity.images[i]))
                        throw new Exception("This is not Images");
                    list_name.Add(await Upload_image(entity.images[i], "products" + entity.data!.Id + $"-{i}", path));
                }
            }
            var t = list_name;
            return t;
  
        }

        public async Task<string> UploadProfile_Image(ImageUpload<Account> entity)//one image
        {
            string path = "avatar";
            if (entity.images == null)
                throw new Exception("Cannot upload null image");
            if (!IsImage(entity.images[0]))
                throw new Exception("This is not Images");
            return await Upload_image(entity.images[0], entity.data!.Id!, path);
        }

        public async Task<string> Upload_image(IFormFile image, string filename,string path)
        {
           string  fullfilename = filename + Path.GetExtension(image.FileName);
            path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{path}", fullfilename);
            //lưu file
            using (var stream = new FileStream(path, FileMode.Create))
            {
                    await Task.Run( async () =>await image.CopyToAsync(stream));
            }
            return fullfilename;
        }

        static bool IsImage(IFormFile image)
        {
            return image.ContentType.StartsWith("image/");
        }
    }
}
