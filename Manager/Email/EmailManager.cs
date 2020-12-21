
using MailKit.Net.Smtp;
using MimeKit;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CLOC.Manager.Email
{
    class EmailManager
    {
        #region Singleton

        private static EmailManager instance; 

         public static EmailManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EmailManager();
                }

                return instance;
            }
        }

        #endregion

        private bool sending;

        private EmailManager()
        {
            sending = false;
        }

        public void SendEmail(string reciverEmail, string subject, string body)
        {
            if (!sending)
                SendEmailAsync(reciverEmail, subject, body);                                  
        }

        private async Task SendEmailAsync(string reciverEmail, string subject, string body)
        {
            try
            {
                sending = true;

                string sender = "developerdevtest251@gmail.com";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("CLOC Sender report", sender));
                message.To.Add(new MailboxAddress(reciverEmail));
                message.Subject = subject;
                message.Body = new TextPart()
                {
                    Text = body
                };

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync("smtp.gmail.com", 587);
                    await client.AuthenticateAsync(sender, "********");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

            }
            catch (System.Exception e)
            {
                Debug.WriteLine($"{GetType().Name} - SendEmailAsync - {e.ToString()}");                    
            }

            sending = false;
        }        
    }
}
