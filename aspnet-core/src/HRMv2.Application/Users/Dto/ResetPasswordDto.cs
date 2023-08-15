using System.ComponentModel.DataAnnotations;

namespace HRMv2.Users.Dto
{
    public class ResetPasswordDto
    {

        [Required]
        public long UserId { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
