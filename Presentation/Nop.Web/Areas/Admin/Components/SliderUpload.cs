using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components
{
    public class SliderUploadViewComponent : NopViewComponent
    {
        private readonly IFileManager _fileManagerModelFactory;

        public SliderUploadViewComponent(IFileManager fileManagerModelFactory)
        {
            _fileManagerModelFactory = fileManagerModelFactory;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(string extensions, string controllerName, string actionName)
        {
            var model = await _fileManagerModelFactory.PrepareFileUploadModelAsync(extensions, 0, 0);
            model.ControllerName = controllerName;
            model.ActionName = actionName;
            return View(model);
        }
    }
}
