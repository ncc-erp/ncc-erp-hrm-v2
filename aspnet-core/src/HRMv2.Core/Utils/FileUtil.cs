using Abp.UI;
using HRMv2.Constants;
using HRMv2.Constants.Dictionary;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Utils
{
    public class FileUtil
    {
        public static void CheckFormatFile(IFormFile file, string[] allowFileTypes)
        {
            if (file == null)
                return;
            var fileExt = Path.GetExtension(file.FileName).Substring(1).ToLower();
            if (!allowFileTypes.Contains(fileExt))
            {
                throw new UserFriendlyException($"Wrong Format! Allow File Type: {allowFileTypes}");
            }
        }
        public static void CheckSizeFile(IFormFile file)
        {
            if (file.Length > UploadFileConstant.MaxSizeFile)
                throw new UserFriendlyException($"File cannot be larger {UploadFileConstant.MaxSizeFile / UploadFileConstant.MEGA_BYTE}MB");
        }
        public static string FullFilePath(string filePath)
        {
            if (String.IsNullOrEmpty(filePath))
                return string.Empty;
            if (UploadFileConstant.UploadFileProvider == UploadFileConstant.AmazoneS3)
            {
                return AmazoneS3Constant.CloudFront.TrimEnd('/') + "/" + filePath;
            }
            else
            {
                return UploadFileConstant.RootUrl.TrimEnd('/') + "/" + filePath;
            }
        }

        public static string GetFileExtension(IFormFile file)
        {
            if (file == default || string.IsNullOrEmpty(file.FileName))
            {
                return "";
            }
            return Path.GetExtension(file.FileName).Substring(1).ToLower();
        }

        public static string GetFileName(string filePath)
        {
            if(filePath == null)
            {
                return "";
            }
            if (filePath.Contains("/"))
            {
                return filePath.Substring(filePath.LastIndexOf("/")+1);
            }
            return filePath;
        }
    }
}
