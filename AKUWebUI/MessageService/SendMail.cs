

using MailKit.Net.Smtp;
using MimeKit;

namespace AKUWebUI.MessageService
{
    public class SendMail
    {
        string receiveEmail;
        string body;
        string subject;
        public SendMail(string receiveEmail, string body, string subject)
        {
            this.receiveEmail = receiveEmail;
            this.body = body;
            this.subject = subject;
        }
        public async Task SendMailAsync(string? att = null)
        {
            MimeKit.MimeMessage mimeMessage = new();
            MimeKit.MailboxAddress from = new("Admin", "velikabuk20@gmail.com");
            mimeMessage.From.Add(from);
            MimeKit.MailboxAddress to = new("client", receiveEmail);
            mimeMessage.To.Add(to);
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = body;
           if(att != null)
				bodyBuilder.Attachments.Add(att);
			mimeMessage.Body = bodyBuilder.ToMessageBody();
            mimeMessage.Subject = subject;
         

            SmtpClient client = new();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("velikabuk20@gmail.com", "eykgxbxaxjexhzwd");

            await client.SendAsync(mimeMessage);
            await client.DisconnectAsync(true);
        }
    }
}

