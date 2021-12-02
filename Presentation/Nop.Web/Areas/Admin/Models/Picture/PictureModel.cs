using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Picture
{
    public class PictureModel
    {
        public string PictureUrl { get; set; }
        public string PictureName { get; set; }
    }


    public class PictureListModel
    {
        public PictureListModel()
        {
            
        }
        public List<PictureModel> pictureModels = new List<PictureModel>();
    }
}
