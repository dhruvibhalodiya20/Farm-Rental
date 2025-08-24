using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;

namespace famrhouserent.Services
{
    public class EmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _password;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<EmailService> logger)
        {
            _logger = logger;
            _smtpServer = configuration["Email:SmtpServer"] ?? throw new ArgumentNullException("SmtpServer");
            _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
            _fromEmail = configuration["Email:FromEmail"] ?? throw new ArgumentNullException("FromEmail");
            _fromName = configuration["Email:FromName"] ?? "FarmHouse Hub";
            _password = configuration["Email:Password"] ?? throw new ArgumentNullException("Password");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                using var client = new SmtpClient
                {
                    Host = _smtpServer,
                    Port = _smtpPort,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_fromEmail, _password),
                    Timeout = 30000 // 30 seconds
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                message.To.Add(toEmail);

                await client.SendMailAsync(message);
                _logger.LogInformation($"Email sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                return false;
            }
        }

        public async Task<bool> SendOtpAsync(string toEmail, string otp)
        {
            var subject = "Your Verification Code - FarmHouse Hub";
            var body = GenerateOtpEmailTemplate(otp);
            return await SendEmailAsync(toEmail, subject, body);
        }

        private string GenerateOtpEmailTemplate(string otp)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <div style='text-align: center; padding: 20px;'>
                        <h1 style='color: #2ecc71;'>FarmHouse Hub</h1>
                    </div>
                    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px;'>
                        <h2 style='color: #333;'>Your Verification Code</h2>
                        <div style='background-color: #ffffff; padding: 15px; border-radius: 5px; 
                                  text-align: center; font-size: 32px; letter-spacing: 5px; 
                                  margin: 20px 0; border: 2px dashed #2ecc71;'>
                            {otp}
                        </div>
                        <p style='color: #666;'>
                            This code will expire in 10 minutes.<br>
                            If you didn't request this code, please ignore this email.
                        </p>
                    </div>
                    <div style='text-align: center; color: #666; font-size: 12px; margin-top: 20px;'>
                        <p>© 2024 FarmHouse Hub. All rights reserved.</p>
                    </div>
                </div>";
        }

        public async Task SendVerificationEmail(string toEmail, string verificationToken)
        {
            var request = _httpContextAccessor.HttpContext!.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var verificationLink = $"{baseUrl}/Verification/Verify?token={verificationToken}";

            var smtpClient = new SmtpClient(_smtpServer)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_fromEmail, _password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = "Email Verification",
                Body = $"Please verify your email by clicking this link: <a href='{verificationLink}'>{verificationLink}</a>",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task<bool> SendOtpEmail(string toEmail, string otp)
        {
            try
            {
                using var smtpClient = new SmtpClient()
                {
                    Host = _smtpServer,
                    Port = _smtpPort,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(_fromEmail, _password),
                    Timeout = 30000
                };

                _logger.LogInformation($"Attempting to send OTP email to {toEmail} using {_fromEmail}");

                var message = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = "Your Verification Code",
                    IsBodyHtml = true,
                    Body = $@"
                        <div style='font-family: Arial, sans-serif; padding: 20px;'>
                            <h2>Your Verification Code: {otp}</h2>
                            <p>This code will expire in 10 minutes.</p>
                        </div>"
                };
                message.To.Add(toEmail);

                // Test SMTP connection first
                try
                {
                    await smtpClient.SendMailAsync(message);
                    _logger.LogInformation($"Successfully sent OTP to {toEmail}");
                    return true;
                }
                catch (SmtpException smtpEx)
                {
                    _logger.LogError(smtpEx, $"SMTP Error sending OTP to {toEmail}. Error: {smtpEx.StatusCode} - {smtpEx.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"General error sending OTP to {toEmail}");
                return false;
            }
        }

        public async Task SendPasswordResetEmail(string toEmail, string resetToken)
        {
            var request = _httpContextAccessor.HttpContext!.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var resetLink = $"{baseUrl}/Auth/ResetPassword?token={resetToken}&email={toEmail}";

            var smtpClient = new SmtpClient(_smtpServer)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_fromEmail, _password),
                EnableSsl = true,
            };

            string emailTemplate = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <div style='text-align: center; margin-bottom: 30px;'>
                        <img src='https://farmhousehub.in/images/farmhousehub-logo.svg' alt='FarmHouse Hub Logo' style='max-width: 200px;'>
                    </div>
                    <div style='background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                        <h1 style='color: #333333; text-align: center; margin-bottom: 20px;'>Password Reset</h1>
                        <p style='color: #666666; font-size: 16px; line-height: 1.5; margin-bottom: 20px;'>
                            You've requested to reset your password. Click the button below to reset it:
                        </p>
                        <div style='text-align: center;'>
                            <a href='{resetLink}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;'>Reset Password</a>
                        </div>
                        <p style='color: #666666; font-size: 14px; text-align: center; margin-top: 20px;'>
                            If you didn't request this reset, please ignore this email.
                        </p>
                    </div>
                    <div style='text-align: center; margin-top: 20px; color: #999999; font-size: 12px;'>
                        <p>© 2024 FarmHouse Hub. All rights reserved.</p>
                    </div>
                </div>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = "FarmHouse Hub - Password Reset",
                Body = emailTemplate,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }

    // Add custom exception class
    public class EmailServiceException : Exception
    {
        public EmailServiceException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
