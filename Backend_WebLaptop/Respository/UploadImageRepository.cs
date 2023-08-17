using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.Respository
{
    public class UploadImageRepository : IUploadImageRepository
    {

        public async Task<List<string>> UploadProduct_Image(ImageUpload imageUpload, string filename)//multi images
        {
            var list_name = new List<string>();
            imageUpload.SetPathSave("Products");
            if (imageUpload.images == null)
                throw new Exception("Cannot upload null image");
            else
            {
                for (int i = 0; i < imageUpload.images.Count; i++)
                {
                    if (!IsImage(imageUpload.images[i]))
                        throw new Exception("This is not Images");
                    list_name.Add(await Upload_image(imageUpload.images[i], "products" + filename + $"-{i}", imageUpload.GetPathSave()));
                }
            }
            var t = list_name;
            return t;
  
        }

        public async Task<string> UploadProfile_Image(ImageUpload imageUpload, string filename)//one image
        {
            imageUpload.SetPathSave("Avatar");
            if (imageUpload.images == null)
                throw new Exception("Cannot upload null image");
            if (!IsImage(imageUpload.images[0]))
                throw new Exception("This is not Images");
            return await Upload_image(imageUpload.images[0], filename, imageUpload.GetPathSave());
        }

        public async Task<string> Upload_image(IFormFile image, string filename,string path)
        {
           string  fullfilename = filename + Path.GetExtension(image.FileName);
            path = Path.Combine(Directory.GetCurrentDirectory(), $"Upload\\Image\\{path}", fullfilename);
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
