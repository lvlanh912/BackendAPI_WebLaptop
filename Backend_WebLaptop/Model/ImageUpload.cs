namespace Backend_WebLaptop.Model
{
    public class ImageUpload<T>
    {
        public List<IFormFile>? Images { get; set; } = null;
        public T? Data { get; set; }
    }
}
