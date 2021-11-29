using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class FileUploadViewComponent : NopViewComponent
    {
        private readonly IFileManager _fileManagerModelFactory;

        public FileUploadViewComponent(IFileManager fileManagerModelFactory)
        {
            _fileManagerModelFactory = fileManagerModelFactory;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(string extensions, int sourceTypeId, int sourceId, int UserId)
        {
            var model = await _fileManagerModelFactory.PrepareFileUploadModelAsync(extensions, sourceTypeId, sourceId);
            model.UserId = UserId;
            return View(model);
        }
    }
}
