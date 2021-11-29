using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Web.Framework.EmailHelper
{
    public interface IEmailService
    {
        Task SendEmailToNotSignInUsers(int quoteId, List<int> userIds);
        Task SendEmail(string toEmail, string toName, string emailSubject, string emailBody);
    }
}
