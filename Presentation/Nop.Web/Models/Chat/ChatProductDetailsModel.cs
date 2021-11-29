using Nop.Web.Framework.Models;
namespace Nop.Web.Models.Common
{
    public partial record ChatProductDetailsModel : BaseNopModel
    {
        public string QuoteGuid { get; set; }
        public bool CustomerFlag { get; set; }
    }
}
