using Nop.Web.Framework.Models;
using Nop.Web.Models.Vendors;
using System.Collections.Generic;

namespace Nop.Web.Models.Common
{
    public partial record ForwardSelectionListModel : BaseNopModel
    {
        public ForwardSelectionListModel()
        {
            ForwardDetailsList = new List<ForwardDetails>();
        }
        public bool IsCustomer { get; set; }
        public int CustomerQouteId { get; set; }
        public List<ForwardDetails> ForwardDetailsList { get; set; }
    }
}
