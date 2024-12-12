using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Silence.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Silence.Web.Controllers
{
    [Authorize]
    [Route("")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpGet("storage/{fileName}")]
        public async Task<IActionResult> Download(string fileName)
        {
            if (ModelState.IsValid)
            {
                var response = await _storageService.Download(fileName);
                if (response.StatusCode == Silence.Infrastructure.Helpers.StatusCode.OK)
                {
                    return File(System.IO.File.ReadAllBytes("/storage/" + fileName), MimeTypes.GetMimeType("/storage/" + fileName));
                }
            }

            return NotFound();
        }

        [HttpPost("storage")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is not selected");
            }

            string fileUID = Guid.NewGuid().ToString();
            string filePath = Path.Combine("/storage", fileUID + "-" + file.FileName);

            if (ModelState.IsValid)
            {
                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                catch
                {
                    return BadRequest();
                }

                return Ok();
            }

            return BadRequest();
        }
    }
}
