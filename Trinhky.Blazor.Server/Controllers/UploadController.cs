using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Trinhky.Blazor.Server.Editors;

namespace Trinhky.Blazor.Server.Controllers
{
    public class ChunkMetadata
    {
        public int Index { get; set; }
        public int TotalCount { get; set; }
        public int FileSize { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public string? FileGuid { get; set; }
    }

    [Route("api/[controller]")]
    //[ApiController]
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        FileUrlStorageService _fileUrlStorageService;
        private string userFolder;
        private string errmsg = "";
        public UploadController(IWebHostEnvironment hostingEnvironment, FileUrlStorageService fileUrlStorageService)
        {
            _hostingEnvironment = hostingEnvironment;
            _fileUrlStorageService = fileUrlStorageService;
        }

        //[HttpPost("[action]")]
        [HttpPost]
        [Route("UploadFile")]
        [DisableRequestSizeLimit]
        public ActionResult Upload(IFormFile ImageUpload, string chunkMetadata, string folder)
        {
            string sothe = Request.Form["sothe"];
            if (!string.IsNullOrEmpty(sothe)) 
            {
                userFolder = @"Files\"+sothe+@"\";
                if (!Directory.Exists(userFolder))
                    Directory.CreateDirectory(userFolder);
            }
            else
                userFolder = @"Files\";
            var tempPath = Path.Combine(_hostingEnvironment.WebRootPath, userFolder);
            // Removes temporary files
            RemoveTempFilesAfterDelay(tempPath, new TimeSpan(0, 5, 0));
            try
            {
                // Write code that saves the 'myFile' file.
                // Don't rely on or trust the FileName property without validation.
                if (!string.IsNullOrEmpty(chunkMetadata))
                {
                    var metaDataObject = JsonConvert.DeserializeObject<ChunkMetadata>(chunkMetadata);
                    //var extension = Path.GetExtension(metaDataObject.FileName).ToLower();
                    var tempFilePath = Path.Combine(tempPath, metaDataObject.FileGuid + ".tmp");
                    if (!Directory.Exists(tempPath))
                        Directory.CreateDirectory(tempPath);

                    AppendContentToFile(tempFilePath, ImageUpload);

                    if (metaDataObject.Index == (metaDataObject.TotalCount - 1))
                    {
                        if (ProcessUploadedFile(tempFilePath, metaDataObject.FileName))
                        {
                            _fileUrlStorageService.Add(Guid.Parse(metaDataObject.FileGuid), userFolder + metaDataObject.FileName);
                        }
                        else
                        {
                            errmsg = "Tệp bị trùng tên";
                            throw new Exception();
                        }
                    }
                }
            }
            catch
            {
                return BadRequest(errmsg);
            }
            return Ok();
        }
        void RemoveTempFilesAfterDelay(string path, TimeSpan delay)
        {
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
                foreach (var file in dir.GetFiles("*.tmp").Where(f => f.LastWriteTimeUtc.Add(delay) < DateTime.UtcNow))
                    file.Delete();
        }
        bool ProcessUploadedFile(string tempFilePath, string fileName)
        {
            var path = Path.Combine(_hostingEnvironment.WebRootPath, userFolder);
            var imagePath = Path.Combine(path, fileName);
            if (System.IO.File.Exists(imagePath))
                return false;
            else
            {
                //System.IO.File.Delete(imagePath);
                System.IO.File.Copy(tempFilePath, imagePath);
                return true;
            }
        }
        void AppendContentToFile(string path, IFormFile content)
        {
            using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                content.CopyTo(stream);
            }
        }

    }
}
