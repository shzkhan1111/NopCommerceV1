using Nop.Web.Framework.Models;
using System;

namespace Nop.Web.Models.Common
{
   
    public partial record ChatListerModel : BaseNopModel
    {
        public ChatListerModel(string _UserName, string _CompanyName, string _Initials, int _QuoteId)
        {
            UserName = _UserName;
            CompanyName = _CompanyName;
            Initials = _Initials;
            QuoteId = _QuoteId;
        }
        public int QuoteId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Initials { get; set; }
        public string CompanyName { get; set; }
        public DateTime QuoteDate { get; set; }
        public bool CustomerFlag { get; set; }
        public bool IsAdmin { get; set; }
        public string QuoteGuid { get; set; }
        public string CustomerQuoteGuid { get; set; }
    }
}
