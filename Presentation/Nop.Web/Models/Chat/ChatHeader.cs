using Nop.Web.Framework.Models;
namespace Nop.Web.Models.Common
{
    public partial record ChatBoxModel : BaseNopModel
    {
        public ChatBoxModel(int userId, int chatId, int vendorId)
        {
            UserId = userId;
            ChatId = chatId;
            VendorId = vendorId;
        }
        public int VendorId { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
    }
}
