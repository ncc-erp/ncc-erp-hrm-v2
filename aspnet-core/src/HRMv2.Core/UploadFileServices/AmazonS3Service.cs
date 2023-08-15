using Abp.UI;
using Amazon.S3;
using Amazon.S3.Model;
using HRMv2.Constants;
using HRMv2.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace HRMv2.UploadFileServices
{
    public class AmazonS3Service : IUploadFileService
    {
        private readonly IAmazonS3 s3Client;

        public AmazonS3Service( IAmazonS3 _s3Client)
        {
            this.s3Client = _s3Client;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string[] allowFileTypes, string filePath)
        {
            FileUtil.CheckSizeFile(file);
            FileUtil.CheckFormatFile(file, allowFileTypes);
            var key = $"{AmazoneS3Constant.Prefix?.TrimEnd('/')}/{filePath}";
            var request = new PutObjectRequest()
            {
                BucketName = AmazoneS3Constant.BucketName,
                Key = key,
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            await s3Client.PutObjectAsync(request);
            return key;
        }


        public async Task<string> UploadAvatar(IFormFile file, string tanentName)
        {
            var filePath = $"{UploadFileConstant.AvatarFolder?.TrimEnd('/')}/{tanentName}/{CommonUtil.NowToYYYYMMddHHmmss()}_{Guid.NewGuid()}.{FileUtil.GetFileExtension(file)}";
            return await UploadFileAsync(file, UploadFileConstant.AllowImageFileTypes, filePath);
        }

        public async Task<string> UploadFiles(IFormFile file, string tenantName, string subFolder)
        {
            var filePath = $"{UploadFileConstant.FileFolder?.TrimEnd('/')}/{tenantName}/{subFolder}/{file.FileName}";
            return await UploadFileAsync(file, UploadFileConstant.AllowFileTypes, filePath);
        }
    }
}
