using AlumniWebsite.API.ModelDto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IHttpContextAccessor accessor;

        public DocumentController(IWebHostEnvironment hostingEnvironment, IHttpContextAccessor accessor)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.accessor = accessor;
        }
        [HttpPost("UploadDocument")]
        public async Task<IActionResult> UploaDocument()
        {
            var result = Request.Form.Files[0];
            string webRootPath = hostingEnvironment.WebRootPath;
            string path = Path.Combine(webRootPath, "Calendar");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (result.Length > 0)
            {
                string filename = result.FileName;
                string fullPath = Path.Combine(path, filename);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await result.CopyToAsync(stream);
                }
            }

            return Ok();
        }
        [HttpDelete("DeleteDocument/{docName}")]
        public ActionResult DeleteDocument([FromRoute] string docName)
        {
            if (docName == null)
                return NotFound("Document name not found");

            string webRootPath = hostingEnvironment.WebRootPath;
            string path = Path.Combine(webRootPath, "Calendar//" + docName);
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
                return NotFound("Document name not found");
            file.Delete();
            return Ok();
        }
        [HttpGet("GetDocumentName")]
        public IActionResult GetDocumentName()
        {
            //string BaseUrl = $"{accessor.HttpContext.Request.Scheme}://{accessor.HttpContext.Request.Host}{accessor.HttpContext.Request.PathBase}/";
            string webroot = hostingEnvironment.WebRootPath;
            string path = Path.Combine(webroot, "Calendar/calendar.pdf");
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            string docstring = Convert.ToBase64String(bytes, 0, bytes.Length);
            return Ok(new { path = docstring });
        }
    }
}
