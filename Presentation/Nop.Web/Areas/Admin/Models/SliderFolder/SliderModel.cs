using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.SliderFolder
{
    public class SliderModel
    {
        public string PictureUrl { get; set; }
        public string PictureName { get; set; }
    }

    public class SliderListModel {
        public SliderListModel()
        {
            //sliderModels = new List<SliderModel>();
        }
        public List<SliderModel> sliderModels = new List<SliderModel>();
    }
}
