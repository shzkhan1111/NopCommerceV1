using System;
using System.Collections.Generic;
using Nop.Core.Domain.Media;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common
{
    public partial record FileListerModel : BaseNopModel
    {
        public FileListerModel(int _sourceTypeId, int _sourceId)
        {
            SourceTypeId = _sourceTypeId;
            SourceId = _sourceId;
            DownloadList = new List<Download>();
        }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> SourceId { get; set; }
        public Nullable<int> SourceTypeId { get; set; }

        public List<Download> DownloadList { get; set; }
        public bool IsAdmin { get; set; }
    }
}