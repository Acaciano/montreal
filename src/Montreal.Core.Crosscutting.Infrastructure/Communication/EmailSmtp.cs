using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Montreal.Core.Crosscutting.Infrastructure.Communication
{
    public class EmailSmtp : IEmailSmtp
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage, SmtpConfig smtpConfig)
        {
            try
            {
                SmtpConfig smtpSettings = smtpConfig;

                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(smtpSettings.Username, smtpSettings.Name)
                };

                mail.To.Add(new MailAddress(email));

                mail.Subject = subject;
                mail.Body = htmlMessage;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(smtpSettings.Domain, smtpSettings.Port))
                {
                    smtp.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
                    smtp.EnableSsl = true;

                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
