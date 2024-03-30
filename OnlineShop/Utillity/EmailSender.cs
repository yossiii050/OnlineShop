using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;

namespace OnlineShop.Utillity
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {
            MailjetClient client = new MailjetClient("a7051a8a42432cda489f2b14318a236e", "01ceaa13ee2d31dd9e198dc30ec1b693")
            {
                BaseAdress= "https://app.mailjet.com/",
            };
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
             .Property(Send.Messages, new JArray {
             new JObject {
              {
               "From",
               new JObject {
                {"Email", "yossiii050@gmail.com"},
                {"Name", "yossi"}
               }
              }, {
               "To",
               new JArray {
                new JObject {
                 {
                  "Email",
                  "yossiii050@gmail.com"
                 }, {
                  "Name",
                  "yossi"
                 }
                }
               }
              }, {
               "Subject",
               subject
              }, {
               "HTMLPart",
               body
              }
             }
                     });
            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
                Console.WriteLine(response.GetData());
            }
            else
            {
                Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
                Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
                Console.WriteLine(response.GetData());
                Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
            }
        }
            }
}
