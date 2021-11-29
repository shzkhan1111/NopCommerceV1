using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Web.Framework.EmailHelper
{
    public class EmailService : IEmailService
    {
        private readonly ICustomerService _customerService;
        private ILocalizationService _localizationService;
        private IGenericAttributeService _genericAttributeService;

        public EmailService(ICustomerService customerService, IGenericAttributeService genericAttributeService)
        {
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
        }

        public async Task SendEmailToNotSignInUsers(int quoteId, List<int> userIds)
        {
            int minutes = int.Parse(await _localizationService.GetResourceAsync("OfflineUser.MinutesDuration"));
            DateTime dateCriteria = DateTime.Now.AddMinutes(minutes);
            var onlineCustomersList = await _customerService.GetOnlineCustomersAsync(dateCriteria, null);
            string emailSubject = await _localizationService.GetResourceAsync("EquipHounds.CustomerService.EmailSubject");
            string emailBody = await _localizationService.GetResourceAsync("EquipHounds.CustomerService.EmailBody");
            emailBody = string.Format(emailBody, quoteId);
            foreach (int userId in userIds)
            {
                var onlineCustomer = onlineCustomersList.Where(x => x.Id == userId).FirstOrDefault();
                
                if (onlineCustomer == null)
                {
                    var customerObj = await _customerService.GetCustomerByIdAsync(userId);
                    string firstName = await _genericAttributeService.GetAttributeAsync<string>(customerObj, Nop.Core.Domain.Customers.NopCustomerDefaults.FirstNameAttribute);
                    string lastName = await _genericAttributeService.GetAttributeAsync<string>(customerObj, Nop.Core.Domain.Customers.NopCustomerDefaults.LastNameAttribute);

                    await SendEmail(customerObj.Email, firstName + " " + lastName, emailSubject, emailBody);
                    //await SendEmail("hamad.mahmood59@gmail.com", firstName + " " + lastName, emailSubject, emailBody);
                }
            }
        }

        public async Task SendEmail(string toEmail, string toName, string emailSubject, string emailBody)
        {
            string apiKey = await _localizationService.GetResourceAsync("SendGrid.apikey");
            string fromEmail = await _localizationService.GetResourceAsync("EquipHounds.CustomerService.Email");
            string fromName = await _localizationService.GetResourceAsync("EquipHounds.CustomerService.Name");
            string htmlContent = "";

            SendGridClient client = new SendGridClient(apiKey);
            EmailAddress from = new EmailAddress(fromEmail, fromName);
            EmailAddress to = new EmailAddress(toEmail, toName);
            SendGridMessage email = MailHelper.CreateSingleEmail(from, to, emailSubject, emailBody, htmlContent);
            Response response = await client.SendEmailAsync(email);
        }


    }
}
