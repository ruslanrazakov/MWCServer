using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.MWCServer;
using MWCServer.Models;
using MWCServer.Services;

namespace MWCServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly PhotosExchange _photosService;

        public PhotoController(ApplicationContext context, PhotosExchange photosService)
        {
            _photosService = photosService;
        }

        // GET: api/Photo
        [HttpGet]
        public OkResult GetPhotos()
        {
            return Ok();
        }

        // GET: api/Photo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetPhoto(long id)
        {
            var photoPath = await _photosService.GetPhotoPath(id);
            return photoPath;
        }

        // POST: api/Photo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Photo>> PostPhoto(Photo photo)
        {
            
            bool successCreated = await _photosService.PostToStorage(photo);
            return successCreated ? CreatedAtAction(nameof(GetPhoto), new { id = photo.Id }, photo.Path) :
                                    BadRequest();
        }
    }
}
