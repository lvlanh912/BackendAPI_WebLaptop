using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using SharpCompress.Common;
using System.IO;

namespace Backend_WebLaptop.Respository
{
    public class UploadImageRepository : IUploadImageRepository
    {

        public async Task<List<string>> UploadProduct_Image(ImageUpload<Product> entity)//multi images
        {
            var listName = new List<string>();
            string path = "products";
            if (entity.Images == null)
                throw new Exception("Cannot upload null image");
            else
            {
                for (int i = 0; i < entity.Images.Count; i++)
                {
                    if (!IsImage(entity.Images[i]))
                        throw new Exception("This is not Images");
                    listName.Add(await Upload_image(entity.Images[i], "products" + entity.Data!.Id + $"-{i}", path));
                }
            }
            var t = listName;
            return t;

        }

        public async Task<string> UploadProfile_Image(ImageUpload<Account> entity)//one image
        {
            string path = "avatar";
            if (entity.Images == null)
                throw new Exception("Cannot upload null image");
            if (!IsImage(entity.Images[0]))
                throw new Exception("This is not Images");
            return await Upload_image(entity.Images[0], entity.Data!.Id!, path);
        }

        public async Task<List<string>> UploadPost_Image(ImageUpload<News> entity)//multi images
        {
            var listName = new List<string>();
            string path = "posts";
            if (entity.Images == null)
                throw new Exception("Cannot upload null image");
            else
            {
                for (int i = 0; i < entity.Images.Count; i++)
                {
                    if (!IsImage(entity.Images[i]))
                        throw new Exception("This is not Images");
                    listName.Add(await Upload_image(entity.Images[i],entity.Data!.Id + $"-{i}", path));
                }
            }
            return listName;
        }

        public async Task<string> Upload_image(IFormFile image, string filename, string path)
        {
            string fullfilename = filename + Path.GetExtension(image.FileName);
            path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{path}", fullfilename);
            //lưu file
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await Task.Run(async () => await image.CopyToAsync(stream));
            }
            return fullfilename;
        }

        static bool IsImage(IFormFile image)
        {
            return image.ContentType.StartsWith("image/");
        }

        public async Task Delete_Image(int type, string namefile)
        {
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\");
                path = type switch
                {
                    //xoá avatar
                    1 => path + "avatar\\" + namefile,
                    //xoá ảnh sản phẩm
                    2 => path + "products\\" + namefile,
                    //xoá ảnh tin đăng
                    3 => path + "posts\\" + namefile,
                    _ => throw new Exception("No type selected"),
                };
                if (File.Exists(path))//kiểm tra file tồn tại hay không
                    await Task.Run(() => new FileInfo(path).Delete());
            }
            catch { }
           
        }

      
    }
}
