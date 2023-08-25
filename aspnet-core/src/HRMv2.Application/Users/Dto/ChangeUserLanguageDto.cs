using System.ComponentModel.DataAnnotations;

namespace HRMv2.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}