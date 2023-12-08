using System.Threading.Tasks;

namespace Montreal.Core.Crosscutting.Infrastructure.Communication
{
    public interface IEmailSmtp
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage, SmtpConfig smtpConfig);
    }
}
