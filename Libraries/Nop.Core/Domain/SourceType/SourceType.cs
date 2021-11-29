using System;

namespace Nop.Core.Domain.SourceType
{
    /// <summary>
    /// Represents a SourceType
    /// </summary>
    public partial class SourceTypes : BaseEntity
    {

        public int Id { get; set; }
        public string SourceTypeTitle { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }

    }
}
