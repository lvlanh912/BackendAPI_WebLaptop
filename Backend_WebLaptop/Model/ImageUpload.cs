namespace Backend_WebLaptop.Model
{
    public class ImageUpload
    {
        public List<IFormFile>? images { get; set; }
        private string Path { get; set; } = "Upload";

        public void SetPathSave(string path)=>this.Path = path;
        public string GetPathSave() => this.Path;
    }
}
