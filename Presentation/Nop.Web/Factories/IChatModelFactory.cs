using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{
    public partial interface IChatModelFactory
    {
        /// <summary>
        /// Prepare the Chat Box Model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the  Chat Box Model
        /// </returns>
        Task<ChatBoxModel> PrepareChatBoxAsync(int UserId, int VendorId, int ChatId);


        /// <summary>
        /// Prepare the Chat Cart Details Model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the  Chat Cart Details Model
        /// </returns>
        Task<ChatCartDetailsModel> PrepareChatCartDetailsAsync();


        /// <summary>
        /// Prepare the Chat Header Model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the  Chat Header Model
        /// </returns>
        Task<ChatHeaderModel> PrepareChatHeaderAsync(string UserName, string CompanyName, string Initials, bool IsAdmin);


        /// <summary>
        /// Prepare the Chat Header Model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the  Chat Header Model
        /// </returns>
        Task<ChatListerModel> PrepareChatListerAsync(string UserName, string CompanyName, string Initials, int QuoteId);
        Task<ChatProductDetailsModel> PrepareChatProductDetailsAsync();
        Task<ForwardSelectionListModel> PrepareForwardSelectionListAsync(int customerQuoteId, bool isCustomer);
    }
}
