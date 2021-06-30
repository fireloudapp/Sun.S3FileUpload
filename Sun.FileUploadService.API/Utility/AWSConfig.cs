using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sun.FileUploadService.API.Utility
{
    public class AWSConfig
    {
        public S3Access S3Access { get; set; }
    }
    public class S3Access
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string S3Region { get; set; }
        public string BucketName { get; set; }
    }
}
