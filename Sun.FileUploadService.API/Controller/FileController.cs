using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sun.FileUploadService.API.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Sun.FileUploadService.API.Controller
{
    /// <summary>
    /// File upload service
    /// </summary>
    [Route("api/FileService/")]
    [ApiController]
    public class FileController : ControllerBase
    {
        ILogger _logger;
        IOptions<AWSConfig> _awsSettings;
        Dictionary<string, string> _extFolder = null;
        //tif tiff png jpg jpeg gif svg
        public FileController(ILogger<FileController> logger, IOptions<AWSConfig> awsSettings)
        {
            _logger = logger;
            _extFolder = new Dictionary<string, string>();
            _extFolder.Add(".png", "png");
            _extFolder.Add(".jpg", "jpg");
            _extFolder.Add(".jpeg", "jpg");
            _extFolder.Add(".tif", "tif");
            _extFolder.Add(".tiff", "tif");
            _extFolder.Add(".ico", "ico");
            _extFolder.Add(".gif", "gif");
            _extFolder.Add(".svg", "svg");
            //awsSettings.Value.S3Access.AccessKey
            _awsSettings = awsSettings;
        }
        /// <summary>
        /// Check if the upload is healthy
        /// </summary>
        /// <returns></returns>
        [Route("IsHealthy")]
        [HttpGet]        
        public IActionResult IsHealthy()
        {
            return Ok();
        }
        
        [Route("Upload")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task< IActionResult> Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                //Request.Headers[""];
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                
                if (file.Length > 0)
                {
                    string fileName, dbPath;
                    string fullPath = SaveFile(file, folderName, pathToSave, out fileName, out dbPath);
                    var extension = Path.GetExtension(fileName);//used to upload file based on extension category.
                    FileMetaData fileMetaData = GetFileMetaData(Request);
                    fileMetaData.Extension = extension;
                    dbPath = await UploadToS3(fileMetaData, dbPath);

                    System.IO.File.Delete(fullPath);//Remove temp file from Server.
                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error : {ex}");
            }
        }

        private async Task<string> UploadToS3(FileMetaData fileMetaData, string dbPath)
        {
            AmazonS3Upload s3Upload = new AmazonS3Upload();
            string bucketName = fileMetaData.ClientName;  //"sat-s3-bucket-poc";
            string buckerFolder = _extFolder.ContainsKey(fileMetaData.Extension) ? _extFolder[fileMetaData.Extension] : "other";//Extension based folder selection.
            //var guid = Guid.NewGuid(); //GUID Disabled.
            buckerFolder = fileMetaData.BranchName + @"/" + buckerFolder;//Bucket folder is storeName
            var s3FileName = fileMetaData.FileName + fileMetaData.Extension;
            var s3result = await s3Upload.SendFileToS3(dbPath, bucketName, buckerFolder, s3FileName, _logger, _awsSettings);
            if (s3result)
            {
                //s3.ap-southeast-1.amazonaws.com
                dbPath = $"https://{bucketName}.{_awsSettings.Value.S3Access.S3Region}/{buckerFolder}/{s3FileName}".ToLower();
                //Example path generation.
                //Ref: https://docs.aws.amazon.com/general/latest/gr/rande.html
                //https://santoyo-s3-bucket-poc.s3.ap-southeast-1.amazonaws.com/png/Asia.png
                //var s3URL = $"https://{bucketName}.s3.ap-southeast-1.amazonaws.com/{buckerFolder}/{fileName}";
                
            }

            return dbPath;
        }

        private static FileMetaData GetFileMetaData(HttpRequest request)
        {
            FileMetaData fileMetaData = new FileMetaData()
            {
                ClientName = request.Headers["clientName"].FirstOrDefault(),
                FileName = request.Headers["fileName"].FirstOrDefault(),
                BranchName = request.Headers["branchName"].FirstOrDefault(),

            };
            return fileMetaData;

        }
        private static string SaveFile(IFormFile file, string folderName, string pathToSave, out string fileName, out string dbPath)
        {
            fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fullPath = Path.Combine(pathToSave, fileName);
            dbPath = Path.Combine(folderName, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return fullPath;
        }

        [Route("Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete(string fileName, string bucketName, string storeName)
        {
            IActionResult ackResult = null;

            try
            {
                AmazonS3Upload s3Upload = new AmazonS3Upload();
                var ext = Path.GetExtension(fileName);
                var extFolder =  _extFolder.ContainsKey(ext) ? _extFolder[ext] : "other";
                ////{storeName}/{extension}/{fileName}
                fileName = storeName + @"/" + extFolder + @"/" + fileName;
                fileName = fileName.ToLower();
                bool result = await s3Upload.DeleteS3File(bucketName, fileName, _logger, _awsSettings);
                if (result)
                {
                    ackResult=  Ok("File Deleted");
                }
                else
                {
                    ackResult = BadRequest("Invalid File/Bucket");
                }

            }
            catch (Exception ex)
            {
                ackResult = StatusCode(500, $"Internal Server Error : {ex}");
            }
            return ackResult;
        }

        [Route("GetObject")]
        [HttpGet]
        public async Task<IActionResult> GetPath(string fileName, string bucketName, string storeName)
        {
            IActionResult ackResult = null;

            try
            {
                AmazonS3Upload s3Upload = new AmazonS3Upload();
                var ext = Path.GetExtension(fileName);
                var extFolder = _extFolder.ContainsKey(ext) ? _extFolder[ext] : "other";
                ////{storeName}/{extension}/{fileName}
                fileName = storeName + @"/" + extFolder + @"/" + fileName;
                fileName = fileName.ToLower();
                string result = await s3Upload.GetS3File(bucketName, fileName, _logger, _awsSettings);                
                ackResult = Ok(result);

            }
            catch (Exception ex)
            {
                ackResult = StatusCode(500, $"Internal Server Error : {ex}");
            }
            return ackResult;
        }
    }

    public class FileMetaData
    {
        public string FileName { get; set; }
        public string ClientName { get; set; }
        public string BranchName { get; set; }
        public string Extension { get; set; }

    }

}
