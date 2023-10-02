namespace Backend_WebLaptop.Model
{
    public class ImageUpload<T>
    {
        public List<IFormFile>? images { get; set; } = null;
        public T? data { get; set; }
    }
}
