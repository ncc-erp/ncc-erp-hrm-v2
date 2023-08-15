using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Constants
{
    public class UploadFileConstant
    {
        public const string APP_NAME = "hrmv2";
        public const long MEGA_BYTE = 1000 * 1000;
        public static string UploadFileProvider { get; set; }
        public static string[] AllowImageFileTypes { get; set; }
        public static readonly string AmazoneS3 = "AWS";
        public static readonly string InternalUploadFile = "Internal";
        public static string RootUrl { get; set; }
        public static long MaxSizeFile { get; set; }
        public static string AvatarFolder { get; set; }
        public static string FileFolder {get;set;}
        public static string[] AllowFileTypes { get; set; }
    }
}
