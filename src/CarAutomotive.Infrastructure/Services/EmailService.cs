using Resend;

namespace CarAutomotive.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;

        public EmailService(IResend resend)
        {
            _resend = resend;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlMessage)
        {
            var message = new EmailMessage
            {
                From = "onboarding@resend.dev",

                To = [toEmail],

                Subject = subject,

                HtmlBody = htmlMessage
            };

            Console.WriteLine("Resend Step 1");

            var response = await _resend.EmailSendAsync(message);

            Console.WriteLine($"Resend Step 2 : {response}");

        }
    }
}