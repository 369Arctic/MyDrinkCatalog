using DrinkCatalog.Services.IService;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace DrinkCatalog.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string SaveImage(IFormFile file, string uploadPath)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, uploadPath, fileName);

            using (var image = Image.Load(file.OpenReadStream()))
            {
                int targetWidth = 300;
                int targetHeight = 300;
                image.Mutate(x => x.Resize(targetWidth, targetHeight));
                var encoder = GetEncoder(file.FileName);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    image.Save(fileStream, encoder);
                }
            }

            return $@"\{uploadPath}\{fileName}";
        }

        public void DeleteImage(string imagePath)
        {
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('\\'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        private IImageEncoder GetEncoder(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            return extension switch
            {
                ".png" => new PngEncoder(),
                ".jpg" or ".jpeg" => new JpegEncoder(),
                ".bmp" => new BmpEncoder(),
                _ => throw new NotSupportedException("Unsupported image format"),
            };
        }
    }

}
