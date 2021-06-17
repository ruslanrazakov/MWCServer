using System;
using System.IO;
using System.Threading.Tasks;
using Data.MWCServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MWCServer.Models;

namespace MWCServer.Services
{
    public class PhotosExchange
    {
        private readonly ApplicationContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger _logger;

        public PhotosExchange(ApplicationContext context, ILogger<PhotosExchange> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }

        public async Task PostToStorage(Photo photo)
        {
            photo.Name = Get8CharacterRandomPhotoName();
            photo.Path = "https://blah-blah/" + photo.Name;
            _context.Photos.Add(photo);
            SavePhotoFromBytesArray(photo);
            await _context.SaveChangesAsync();
        }

		public string Get8CharacterRandomPhotoName()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path.Substring(0, 8) + ".jpg";  // Return 8 character string
        }

		public async Task<string> GetPhotoPath(long id)
        {
            var photo = await _context.Photos.FindAsync(id);

            if (photo == null)
            {
                return null;
            }

            return photo.Path;
        }

        private void SavePhotoFromBytesArray(Photo photo)
        {
            var iiiiiiiiii = _env.WebRootPath + "/photos/" + photo.Name;
            using(var ms = new MemoryStream(Convert.FromBase64String(photo.Content))) {
                using(var fs = new FileStream(_env.WebRootPath + "/" + photo.Name, FileMode.Create)) {
                    ms.WriteTo(fs);
                }
            }
        }
    }
}