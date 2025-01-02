using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Utilities
{
    public static class ImageSaver
    {
        public async static Task<string> SaveImage(IFormFile image)
        {
            /*if (image == null || image.Length == 0)
            {
                return null;
            }*/

            var directoryPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "cafeteria-front\\public\\images\\products");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath); // Klasörü oluştur
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(directoryPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return filePath;
        }
    }
}
