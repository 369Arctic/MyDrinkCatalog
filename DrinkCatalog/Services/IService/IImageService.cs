namespace DrinkCatalog.Services.IService
{
    public interface IImageService
    {
        string SaveImage(IFormFile file, string uploadPath);
        void DeleteImage(string imagePath);
    }
}
