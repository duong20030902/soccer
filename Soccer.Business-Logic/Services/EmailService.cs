using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Soccer.Business_Logic.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendConfirmationEmail(string email, string confirmationLink)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtpSettings["FromName"], smtpSettings["Username"]));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Xác nhận đăng ký tài khoản";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = EmailTemplate.GetConfirmationEmail(confirmationLink)
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            // Kết nối SSL
            await client.ConnectAsync(smtpSettings["Host"], 587, MailKit.Security.SecureSocketOptions.StartTls);

            // Xác thực
            await client.AuthenticateAsync(smtpSettings["Username"], smtpSettings["Password"]);

            // Gửi email
            await client.SendAsync(message);

            // Ngắt kết nối
            await client.DisconnectAsync(true);
        }

        public async Task SendPasswordResetEmail(string email, string resetLink)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtpSettings["FromName"], smtpSettings["Username"]));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Đặt lại mật khẩu";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = EmailTemplate.GetPasswordResetEmail(resetLink)
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            // Kết nối SSL
            await client.ConnectAsync(smtpSettings["Host"], 587, MailKit.Security.SecureSocketOptions.StartTls);

            // Xác thực
            await client.AuthenticateAsync(smtpSettings["Username"], smtpSettings["Password"]);

            // Gửi email
            await client.SendAsync(message);

            // Ngắt kết nối
            await client.DisconnectAsync(true);
        }
    }

    public static class EmailTemplate
    {
        public static string GetConfirmationEmail(string confirmationLink)
        {
            return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                .container {{ max-width: 600px; margin: 20px auto; padding: 30px; background: #f7f9fc; }}
                .logo {{ text-align: center; margin-bottom: 20px; }}
                .content {{ background: white; padding: 40px; border-radius: 10px; }}
                .button {{ background: #2563eb; color: white!important; padding: 12px 25px; 
                          border-radius: 5px; text-decoration: none; display: inline-block; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='logo'>
                    <img src='https://your-logo-url' alt='Logo' width='150'>
                </div>
                <div class='content'>
                    <h2>Cảm ơn bạn đã đăng ký!</h2>
                    <p>Vui lòng nhấp vào nút bên dưới để hoàn tất xác nhận email:</p>
                    <p><a href='{confirmationLink}' class='button'>Xác nhận Email</a></p>
                    <p>Nếu nút không hoạt động, hãy sao chép link này vào trình duyệt:<br>
                    <small>{confirmationLink}</small></p>
                </div>
            </div>
        </body>
        </html>";
        }

        public static string GetPasswordResetEmail(string resetLink)
        {
            return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                .container {{ max-width: 600px; margin: 20px auto; padding: 30px; background: #f7f9fc; }}
                .logo {{ text-align: center; margin-bottom: 20px; }}
                .content {{ background: white; padding: 40px; border-radius: 10px; }}
                .button {{ background: #2563eb; color: white!important; padding: 12px 25px; 
                          border-radius: 5px; text-decoration: none; display: inline-block; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='logo'>
                    <img src='https://your-logo-url' alt='Logo' width='150'>
                </div>
                <div class='content'>
                    <h2>Yêu cầu đặt lại mật khẩu</h2>
                    <p>Bạn đã yêu cầu đặt lại mật khẩu cho tài khoản của mình. Vui lòng nhấp vào nút bên dưới để đặt lại mật khẩu:</p>
                    <p><a href='{resetLink}' class='button'>Đặt lại mật khẩu</a></p>
                    <p>Nếu nút không hoạt động, hãy sao chép link này vào trình duyệt:<br>
                    <small>{resetLink}</small></p>
                    <p>Nếu bạn không yêu cầu đặt lại mật khẩu, bạn có thể bỏ qua email này.</p>
                    <p>Lưu ý: Liên kết đặt lại mật khẩu chỉ có hiệu lực trong 24 giờ.</p>
                </div>
            </div>
        </body>
        </html>";
        }
    }
}
