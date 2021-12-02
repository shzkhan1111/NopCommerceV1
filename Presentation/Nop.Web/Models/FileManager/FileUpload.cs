using System;
using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common
{
    public partial record FileUploadModel : BaseNopModel
    {

        public FileUploadModel(List<string> _allowedFileExtensions, int _sourceTypeId, int _sourceId)
        {
            if (_allowedFileExtensions.Count > 0)
                AllowedFileExtensions = _allowedFileExtensions;
            else
                AllowedFileExtensions = new List<string>();

            SourceId = _sourceId;
            SourceTypeId = _sourceTypeId;
        }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> SourceId { get; set; }
        public Nullable<int> SourceTypeId { get; set; }
        public List<string> AllowedFileExtensions { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }


    }
}