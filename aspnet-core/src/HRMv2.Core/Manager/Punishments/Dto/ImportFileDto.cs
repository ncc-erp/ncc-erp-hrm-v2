using Microsoft.AspNetCore.Http;

namespace HRMv2.Manager.Punishments.Dto
{
    public class ImportFileDto
    {
        public IFormFile File { get; set; }
        public long PunishmentId { get; set; }
    }

    public class ResponseFailDto
    {
        public int Row { get; set; }
        public string Email { get; set; }
        public string Money { get; set; }
        public string ReasonFail { get; set; }
    }
}