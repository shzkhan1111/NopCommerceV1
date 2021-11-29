using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Quote
{
    public class QuoteAttributeMapping : BaseEntity
    {
        public int AttributeId { get; set; }
        public int QuoteId { get; set; }
        public string Value { get; set; }
    }

    public class QuoteAttribueMappingModel
    {
        public int Id { get; set; }
        public int AttributeId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int QuoteId { get; set; }
    }
}
