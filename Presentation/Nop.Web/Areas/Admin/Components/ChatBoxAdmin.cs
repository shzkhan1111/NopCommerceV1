

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components
{
    public class ChatBoxAdminViewComponent : NopViewComponent
    {
        private readonly IChatModelFactory _chatModelFactory;

        public ChatBoxAdminViewComponent(IChatModelFactory chatModelFactory)
        {
            _chatModelFactory = chatModelFactory;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(int UserId, int VendorId, int ChatId)
        {
            var model = await _chatModelFactory.PrepareChatBoxAsync(UserId, VendorId, ChatId);
            //ChatBoxModel model = new ChatBoxModel(UserId, ChatId);
            return View(model);
        }
    }
}
