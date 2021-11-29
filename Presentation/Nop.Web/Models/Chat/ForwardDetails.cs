using Nop.Core.Domain.Media;
using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace Nop.Web.Models.Common
{
    public partial class ForwardDetails
    {
        public ForwardDetails()
        {
            FileList = new List<Download>();
        }
        public int Id { get; set; }
        public int QuoteId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public List<Download> FileList { get; set; }
    }
}
