using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.MezonTokens.Dto
{
    public class TransferTokenMezonDongDto
    {
        public string senderAddress { get; set; }
        public string senderId { get; set; }
        public string toUserId { get; set; }
        public double transferAmount { get; set; }
        public string zkPub { get; set; }
        public string zkProof { get; set; }
        public string note { get; set; }
        public string publicKeyBase58 { get; set; }
        public byte[] privateKeySeed { get; set; }
    }
}
