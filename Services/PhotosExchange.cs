using System;
using System.IO;
using System.Linq;
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

        public async Task<bool> PostToStorage(Photo photo)
        {
            string photoName = Get8CharacterRandomPhotoName();
            while(_context.Photos.Any(p=>p.Name == photo.Name))
                photo.Name = Get8CharacterRandomPhotoName();
            photo.Name = photoName;

            photo.Path = "http://mwc-server.eastus.cloudapp.azure.com/" + photo.Name;
            _context.Photos.Add(photo);

            if(SavePhotoFromBytesArray(photo))
            {
                await _context.SaveChangesAsync();
                _logger.Log(LogLevel.Information, $"{photo.Name} сохранена по адресу {photo.Path}");
                return true;
            }
            else
                return false;
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

        private bool SavePhotoFromBytesArray(Photo photo)
        {
            try
            {
                using(var ms = new MemoryStream(Convert.FromBase64String(photo.Content))) {
                    using(var fs = new FileStream(_env.WebRootPath + "/" + photo.Name, FileMode.Create)) {
                        ms.WriteTo(fs);
                    }
                }
                return true;
            }
            catch
            {
                _logger.Log(LogLevel.Critical, "Строка в секции Content неверного формата! Сохранение прервано");
                _logger.Log(LogLevel.Critical, $"{photo.Content}");
                return false;
            }
        }
    }
}