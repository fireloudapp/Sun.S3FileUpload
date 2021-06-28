using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sun.FileUploadService.API.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class AmazonS3Upload
    {
        public bool CreateS3Bucket(string bucketName)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bucketName"></param>
        /// <param name="subDirectoryInBucket"></param>
        /// <param name="fileNameInS3"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<bool> SendFileToS3(string filePath, string bucketName, string subDirectoryInBucket, string fileNameInS3, ILogger logger, IOptions<AWSConfig> awsConfig)
        {
            Stream localFilePath = null;
            TransferUtility utility = null;
            IAmazonS3 client = null;
            try
            {
                localFilePath = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                string accessKey = awsConfig.Value.S3Access.AccessKey;
                string secretKey = awsConfig.Value.S3Access.SecretKey;
                client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.APSoutheast1);
                IAmazonS3 s3Client = client;
                await CreateBucketAsync(s3Client, bucketName);//Create S3 Bucket if bucket is not available.

                utility = new TransferUtility(client);
                TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
                //Convert to all lower case as S3 object is case sensitive
                filePath = filePath.ToLower();
                subDirectoryInBucket = subDirectoryInBucket.ToLower();
                bucketName = bucketName.ToLower();

                if (subDirectoryInBucket == "" || subDirectoryInBucket == null)
                {
                    request.BucketName = bucketName; //no subdirectory just bucket name  
                }
                else
                {   // subdirectory and bucket name  
                    request.BucketName = bucketName + @"/" + subDirectoryInBucket;
                }
                request.Key = fileNameInS3.ToLower(); //Key is case sensitive hence follow all lower cases.
                request.InputStream = localFilePath;
                request.CannedACL = S3CannedACL.PublicRead;
                
                utility.Upload(request); //commensing the transfer  
                
                logger.LogInformation("Uploaded to S3 Bucket");

                return true; //indicate that the file was sent  
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to Upload S3", null);
                return false;
            }
            finally
            {
                localFilePath?.Dispose();
                utility?.Dispose();
                client?.Dispose();
            }

        }

        static async Task CreateBucketAsync(IAmazonS3 s3Client, string bucketName)
        {
            try
            {
                if (!(await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName)))
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,                        
                    };

                    PutBucketResponse putBucketResponse = await s3Client.PutBucketAsync(putBucketRequest);
                }
                // Retrieve the bucket location.
                string bucketLocation = await FindBucketLocationAsync(s3Client, bucketName);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }

        static async Task<string> FindBucketLocationAsync(IAmazonS3 client, string bucketName)
        {
            string bucketLocation;
            var request = new GetBucketLocationRequest()
            {
                BucketName = bucketName
            };
            GetBucketLocationResponse response = await client.GetBucketLocationAsync(request);
            bucketLocation = response.Location.ToString();
            return bucketLocation;
        }

        public async Task<bool> DeleteS3File(string bucketName, string fileName, ILogger logger, IOptions<AWSConfig> awsConfig)
        {
            bool isDeleted = false;
            IAmazonS3 client = null;
            try
            {
                string accessKey = awsConfig.Value.S3Access.AccessKey;
                string secretKey = awsConfig.Value.S3Access.SecretKey;
                client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.APSoutheast1);
                
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName.ToLower()//Key is case sensitive hence follow Lower case always
                };

                //To DBUG
                //var s3ObjList = await client.GetAllObjectKeysAsync(bucketName, null, null);
                var s3Object = await client.GetObjectAsync(bucketName, fileName);

                Console.WriteLine("Deleting an object");
                await client.DeleteObjectAsync(deleteObjectRequest);
                isDeleted = true;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when deleting an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when deleting an object", e.Message);
            }
            finally
            {
                client?.Dispose();
            }
            return isDeleted;
        }

        public async Task<string> GetS3File(string bucketName, string fileKey, ILogger logger, IOptions<AWSConfig> awsConfig)
        {
            string s3Path = string.Empty;
            IAmazonS3 client = null;
            try
            {
                string accessKey = awsConfig.Value.S3Access.AccessKey;
                string secretKey = awsConfig.Value.S3Access.SecretKey;
                client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.APSoutheast1);

                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileKey.ToLower()
                };

                //To DBUG
                //var s3ObjList = await client.GetAllObjectKeysAsync(bucketName, null, null);
                var s3Object = await client.GetObjectAsync(bucketName, fileKey);
                s3Path = $"https://{bucketName}.{awsConfig.Value.S3Access.S3Region}/{s3Object.Key}".ToLower();


            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when deleting an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when deleting an object", e.Message);
            }
            finally
            {
                client?.Dispose();
            }
            return s3Path;
        }
    }
}
