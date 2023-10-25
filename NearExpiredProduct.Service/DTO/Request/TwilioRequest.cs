using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class TwilioRequest
    {
        public string AccountSID { get; set; }
        public string AuthToken { get; set; }
        public string PathServiceSid { get; set; }
        public string Phone { get; set; }
        public string? Token { get; set; }
    }
}
