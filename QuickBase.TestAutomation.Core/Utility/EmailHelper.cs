//Utility class for sending emails with NUnit test reports.

#region MS Directives
using System.Net;
using System.Net.Mail;
#endregion


namespace QuickBase.TestAutomationCore.Utility
{
    /// <summary>
    /// Utility class for sending emails with NUnit test reports.
    /// </summary>
    public class EmailHelper
    {
        /// <summary>
        /// Sends an email with the NUnit test report attached.
        /// </summary>
        public static void SendEmailWithReport()
        {
            var fromAddress = new MailAddress("automationquickbase@gmail.com");
            var toAddress = new MailAddress("automationquickbase@gmail.com");
            const string subject = "NUnit Test Results";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, "gouh gfye hmiq fnxj")
            };
            Attachment attachment = new Attachment(Constants.HtmlReportPath);

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = "Please find attached the NUnit test report.",
                IsBodyHtml = true,
                Attachments = { attachment }
            })
            {
                try
                {
                    smtp.Send(message);
                    Console.WriteLine("Email sent successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email: {ex.Message}");
                }
            }

        }

    }

}
