using HRMv2.Constants;
using HRMv2.Helper;
using HRMv2.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.UploadFileServices
{
    public class InternalUploadFileService : IUploadFileService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public InternalUploadFileService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<string> UploadAvatar(IFormFile file, string tenantName)
        {
            FileUtil.CheckSizeFile(file);
            FileUtil.CheckFormatFile(file, UploadFileConstant.AllowImageFileTypes);
            var avatarFolder = Path.Combine(_hostingEnvironment.WebRootPath, UploadFileConstant.AvatarFolder, tenantName);
            UploadFile.CreateFolderIfNotExists(avatarFolder);
            var fileName = $"{CommonUtil.NowToYYYYMMddHHmmss()}_{Guid.NewGuid()}.{FileUtil.GetFileExtension(file)}";
            var filePath = $"{UploadFileConstant.AvatarFolder.TrimEnd('/')}/{tenantName}/{fileName}";
            await UploadFileAsync(avatarFolder, file, fileName);
            return filePath;
        }

        public static async Task<string> UploadFileAsync(string fileLocation, IFormFile file, string fileName)
        {
            var fullFilePath = Path.Combine(fileLocation, fileName);
            using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return fileName;
        }

        public async Task<string> UploadFiles(IFormFile file, string tenantName, string subFolder)
        {
            FileUtil.CheckSizeFile(file);
            FileUtil.CheckFormatFile(file, UploadFileConstant.AllowFileTypes);
            var fileFolder = Path.Combine(_hostingEnvironment.WebRootPath, UploadFileConstant.FileFolder, tenantName, subFolder);
            UploadFile.CreateFolderIfNotExists(fileFolder);
            var fileName = $"{file.FileName}";
            var filePath = $"{UploadFileConstant.FileFolder.TrimEnd('/')}/{tenantName}/{subFolder}/{fileName}";
            await UploadFileAsync(fileFolder, file, fileName);
            return filePath;
        }
    }
}
