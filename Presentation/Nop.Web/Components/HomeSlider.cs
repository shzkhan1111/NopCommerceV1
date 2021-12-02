using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Areas.Admin.Models.SliderFolder;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Nop.Core;

namespace Nop.Web.Components
{
    public class HomeSliderViewComponent : NopViewComponent
    {
        private readonly IWebHelper _webHelper;
        public HomeSliderViewComponent(IWebHelper webHelper)
        {
            this._webHelper = webHelper;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            SliderListModel Model = SliderPictureList();
            return View(Model);
        }

        public virtual SliderListModel SliderPictureList()
        {

            //path = System.IO.Directory.GetCurrentDirectory()
            //var files = Directory.GetFiles("wwwroot/images/slider" , "*.jpg");
            //List<string> jpg = Directory.GetFiles("wwwroot/images/ProductImages", "*.jpg").ToList();
            //List<string> png = Directory.GetFiles("wwwroot/images/ProductImages", "*.png").ToList();
            //List<string> svg = Directory.GetFiles("wwwroot/images/ProductImages", "*.svg").ToList();
            List<string> jpg = Directory.GetFiles("wwwroot/images/slider", "*.jpg").ToList();
            List<string> png = Directory.GetFiles("wwwroot/images/slider", "*.png").ToList();
            List<string> svg = Directory.GetFiles("wwwroot/images/slider", "*.svg").ToList();
            List<string> files = new List<string>();
            files.AddRange(jpg);
            files.AddRange(png);
            files.AddRange(svg);

            List<SliderModel> sliderModels = new List<SliderModel>();
            foreach (string file in files)
            {
                SliderModel temp = new SliderModel();
                temp.PictureUrl = GetImagesPathUrlAsync(file);
                temp.PictureName = GetImageName(file);
                sliderModels.Add(temp);
            }
            SliderListModel sliderListModel = new SliderListModel();
            sliderListModel.sliderModels = sliderModels;

            //files = File

            //var files = Directory.GetFiles("wwwroot/images/slider").Where(file => Regex.IsMatch(file, @" ^.+\.(jpg|png|svg)$"));

            //var files = Directory.EnumerateFiles("~/wwwroot/images/slider", "*.*", SearchOption.AllDirectories)
            //.Where(s => s.EndsWith(".png") || s.EndsWith(".jpg"));
            return sliderListModel;
        }
        protected virtual string GetImageName(string storeLocation = null)
        {
            string result = "";
            if (storeLocation.LastIndexOf('\\') != -1)
            {
                result = storeLocation.Substring(storeLocation.LastIndexOf('\\') + 1);
            }
            else
            {
                result = storeLocation.Substring(storeLocation.LastIndexOf('/') + 1);
            }

            return result;
        }
        protected virtual string GetImagesPathUrlAsync(string storeLocation = null)
        {
            //var pathBase = _httpContextAccessor.HttpContext.Request.PathBase.Value ?? string.Empty;
            //var imagesPathUrl = _mediaSettings.UseAbsoluteImagePath ? storeLocation : $"{pathBase}/";
            //imagesPathUrl = string.IsNullOrEmpty(imagesPathUrl) ? _webHelper.GetStoreLocation() : imagesPathUrl;
            string path = AppDomain.CurrentDomain.BaseDirectory;
            var imagesPathUrl = _webHelper.GetStoreLocation();
            string result;
            if (storeLocation.LastIndexOf('\\') != -1)
            {
                result = storeLocation.Substring(storeLocation.LastIndexOf('\\') + 1);
            }
            else
            {
                result = storeLocation.Substring(storeLocation.LastIndexOf('/') + 1);
            }
            imagesPathUrl = imagesPathUrl + "images/slider/" + result;
            //return Task.FromResult(imagesPathUrl);
            return imagesPathUrl;
        }
    }
}