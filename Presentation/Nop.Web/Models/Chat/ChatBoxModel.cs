using Nop.Web.Framework.Models;
using System;

namespace Nop.Web.Models.Common
{
    public partial record ChatHeaderModel : BaseNopModel
    {
        public ChatHeaderModel(string _UserName, string _CompanyName, string _Initials, bool _IsAdmin)
        {
            UserName = _UserName;
            CompanyName = _CompanyName;
            Initials = _Initials;
            IsAdmin = _IsAdmin;
        }
        public string UserName { get; set; }
        public string Initials { get; set; }
        public string CompanyName { get; set; }
        public string QuoteGuid { get; set; }
        public string CustomerQuoteGuid { get; set; }
        public bool IsAdmin { get; set; }
        public int QuoteId { get; set; }
        public DateTime QuoteDate { get; set; }
        public bool CustomerFlag { get; set; }
    }
}
