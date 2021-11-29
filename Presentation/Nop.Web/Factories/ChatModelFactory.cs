using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{
    public partial class ChatModelFactory : IChatModelFactory
    {

        /// <summary>
        /// Prepare the chatbox model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the chatbox model
        /// </returns>
        public virtual async Task<ChatBoxModel> PrepareChatBoxAsync(int UserId, int VendorId, int ChatId)
        {
            return new ChatBoxModel(UserId, VendorId, ChatId);
        }

        /// <summary>
        /// Prepare the Chat Cart Details model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the Chat Cart Details model
        /// </returns>
        public virtual async Task<ChatCartDetailsModel> PrepareChatCartDetailsAsync()
        {
            return new ChatCartDetailsModel();
        }


        /// <summary>
        /// Prepare the Chat header model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the Chat header model
        /// </returns>
        public virtual async Task<ChatHeaderModel> PrepareChatHeaderAsync(string UserName, string CompanyName, string Initials, bool IsAdmin)
        {
            return new ChatHeaderModel(UserName, CompanyName, Initials, IsAdmin);
        }


        /// <summary>
        /// Prepare the Chat lister model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the Chat lister model
        /// </returns>
        public virtual async Task<ChatListerModel> PrepareChatListerAsync(string UserName, string CompanyName, string Initials, int QuoteId)
        {
            return new ChatListerModel(UserName, CompanyName, Initials, QuoteId);
        }
        public virtual async Task<ChatProductDetailsModel> PrepareChatProductDetailsAsync()
        {
            return new ChatProductDetailsModel();
        }

        public virtual async Task<ForwardSelectionListModel> PrepareForwardSelectionListAsync(int customerQuoteId, bool isCustomer)
        {
            var fwdListModel = new ForwardSelectionListModel();
            fwdListModel.CustomerQouteId = customerQuoteId;
            fwdListModel.IsCustomer = isCustomer;
            return fwdListModel;
        }

    }
}
