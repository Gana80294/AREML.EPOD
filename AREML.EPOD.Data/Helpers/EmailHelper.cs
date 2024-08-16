using AREML.EPOD.Data.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Data.Helpers
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;

        public EmailHelper(IConfiguration configuration) 
        { 
            _configuration = configuration;
        }

        public async Task<bool> SendMail(string code, string UserName, string toEmail, string password, string type, string userID, string siteURL)
        {
            try
            {
                string hostName = _configuration["HostName"];
                string SMTPEmail = _configuration["SMTPEmail"];
                //string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
                string SMTPEmailPassword =_configuration["SMTPEmailPassword"];
                string SMTPPort = _configuration["SMTPPort"];
                var message = new MailMessage();
                string subject = "";
                StringBuilder sb = new StringBuilder();
                //string UserName = _ctx.TBL_User_Master.Where(x => x.Email == toEmail).Select(y => y.UserName).FirstOrDefault();
                //UserName = string.IsNullOrEmpty(UserName) ? toEmail.Split('@')[0] : UserName;
                sb.Append(string.Format("Dear {0},<br/>", UserName));
                if (type == "ConfirmEmail")
                {

                    sb.Append("<p>Thank you for subscribing to Amararaja POD Confirmation.</p>");
                    sb.Append("<p>Your sign in details have been authorised by us so you just need to verify your account by clicking <a href=\"" + siteURL + "/#/login?token=" + code + "&Id=" + userID + "\"" + ">here</a>. Once this has been done you will be redirected to the log in page where you can enter your credentials and start completing your profile.</p>");
                    sb.Append(String.Format("<p>User name: {0}</p>", toEmail));
                    sb.Append(String.Format("<p>Password: {0}</p>", password));
                    sb.Append("<i>Note: The verification link will expire in 2 hours.<i>");
                    sb.Append("<p>Regards,</p><p>Admin</p>");
                    subject = "Account verification";
                }
                else
                {
                    sb.Append("<p>We have received a request to reset your password, you can reset it now by clicking <a href=\"" + siteURL + "?token=" + code + "&Id=" + userID + "\"" + ">here</a>.<p></p></p>");
                    sb.Append("<p>Regards,</p><p>Admin</p>");
                    subject = "Amararaja POD - Reset password";
                }
                SmtpClient client = new SmtpClient();
                client.Port = Convert.ToInt32(SMTPPort);
                client.Host = hostName;
                client.EnableSsl = false;
                client.Timeout = 60000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(SMTPEmail, SMTPEmailPassword);
                MailMessage reportEmail = new MailMessage(SMTPEmail, toEmail, subject, sb.ToString());
                reportEmail.BodyEncoding = UTF8Encoding.UTF8;
                reportEmail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                reportEmail.IsBodyHtml = true;
                //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                await client.SendMailAsync(reportEmail);
                LogWriter.WriteToFile($"Reset password link has been sent successfully to {toEmail}");
                return true;
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        LogWriter.WriteToFile("Master/SendMail/MailboxBusy/MailboxUnavailable/SmtpFailedRecipientsException:Inner- " + ex.InnerExceptions[i].Message);
                    }
                    else
                    {
                        LogWriter.WriteToFile("Master/SendMail/SmtpFailedRecipientsException:Inner- " + ex.InnerExceptions[i].Message);
                    }
                }
                LogWriter.WriteToFile("Master/SendMail/SmtpFailedRecipientsException:- " + ex.Message, ex);
                return false;
            }
            catch (SmtpException ex)
            {
                LogWriter.WriteToFile("Master/SendMail/SmtpException:- " + ex.Message, ex);
                return false;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("Master/SendMail/Exception:- " + ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> SendOTPMail(string otp, string UserName, string toEmail)
        {
            try
            {
                string hostName = _configuration["HostName"];
                string SMTPEmail = _configuration["SMTPEmail"];
                //string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
                string SMTPEmailPassword = _configuration["SMTPEmailPassword"];
                string SMTPPort = _configuration["SMTPPort"];
                var message = new MailMessage();
                string subject = "";
                StringBuilder sb = new StringBuilder();
                //string UserName = _ctx.TBL_User_Master.Where(x => x.Email == toEmail).Select(y => y.UserName).FirstOrDefault();
                //UserName = string.IsNullOrEmpty(UserName) ? toEmail.Split('@')[0] : UserName;
                sb.Append(string.Format("Dear {0},<br/>", UserName));
                sb.Append($"<p>We have received a request to reset your password, you can reset it now by using the OTP <b>{otp}</b><p></p></p>");
                sb.Append("<p>Regards,</p><p>Admin</p>");
                subject = "Amararaja POD - OTP to Reset password";
                SmtpClient client = new SmtpClient();
                client.Port = Convert.ToInt32(SMTPPort);
                client.Host = hostName;
                client.EnableSsl = false;
                client.Timeout = 60000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(SMTPEmail, SMTPEmailPassword);
                MailMessage reportEmail = new MailMessage(SMTPEmail, toEmail, subject, sb.ToString());
                reportEmail.BodyEncoding = UTF8Encoding.UTF8;
                reportEmail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                reportEmail.IsBodyHtml = true;
                //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                await client.SendMailAsync(reportEmail);
                LogWriter.WriteToFile($"OTP has been sent successfully to {toEmail}");
                return true;
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        LogWriter.WriteToFile("Master/SendMail/MailboxBusy/MailboxUnavailable/SmtpFailedRecipientsException:Inner- " + ex.InnerExceptions[i].Message);
                    }
                    else
                    {
                        LogWriter.WriteToFile("Master/SendMail/SmtpFailedRecipientsException:Inner- " + ex.InnerExceptions[i].Message);
                    }
                }
                LogWriter.WriteToFile("Master/SendMail/SmtpFailedRecipientsException:- " + ex.Message, ex);
                return false;
            }
            catch (SmtpException ex)
            {
                LogWriter.WriteToFile("Master/SendMail/SmtpException:- " + ex.Message, ex);
                return false;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("Master/SendMail/Exception:- " + ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> SendMailToUser(string toEmail, string UserName, string password)
        {
            try
            {
                string hostName = _configuration["HostName"];
                string SMTPEmail = _configuration["SMTPEmail"];
                //string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
                string SMTPEmailPassword = _configuration["SMTPEmailPassword"];
                string SMTPPort = _configuration["SMTPPort"];
                string siteURL = _configuration["SiteURL"];
                var message = new MailMessage();
                string subject = "";
                StringBuilder sb = new StringBuilder();
                //string UserName = _dbContext.TBL_User_Master.Where(x => x.Email == toEmail).Select(y => y.UserName).FirstOrDefault();
                //UserName = string.IsNullOrEmpty(UserName) ? toEmail.Split('@')[0] : UserName;
                sb.Append(string.Format("Dear {0},<br/>", UserName));
                sb.Append("<p>Your account has been created in Amararaja POD Confirmation.</p>");
                sb.Append("<p>Please Login by clicking <a href=\"" + siteURL + "/#/auth/login\">here</a></p>");
                sb.Append(string.Format("<p>User name: {0}</p>", UserName));
                sb.Append(string.Format("<p>Password: {0}</p>", password));
                sb.Append("<p>Regards,</p><p>Admin</p>");
                subject = "POD Confirmation User Creation";
                SmtpClient client = new SmtpClient();
                client.Port = Convert.ToInt32(SMTPPort);
                client.Host = hostName;
                client.EnableSsl = false;
                client.Timeout = 60000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(SMTPEmail.Trim(), SMTPEmailPassword.Trim());
                MailMessage reportEmail = new MailMessage(SMTPEmail, toEmail, subject, sb.ToString());
                reportEmail.BodyEncoding = UTF8Encoding.UTF8;
                reportEmail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                reportEmail.IsBodyHtml = true;
                //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                await client.SendMailAsync(reportEmail);
                LogWriter.WriteToFile($"Mail has been sent successfully to {toEmail}");
                return true;
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        LogWriter.WriteToFile("Master/SendMail/MailboxBusy/MailboxUnavailable/SmtpFailedRecipientsException:Inner- " + ex.InnerExceptions[i].Message);
                    }
                    else
                    {
                        LogWriter.WriteToFile("Master/SendMail/SmtpFailedRecipientsException:Inner- " + ex.InnerExceptions[i].Message);
                    }
                }
                LogWriter.WriteToFile("Master/SendMail/SmtpFailedRecipientsException:- " + ex.Message, ex);
                return false;
            }
            catch (SmtpException ex)
            {
                LogWriter.WriteToFile("Master/SendMail/SmtpException:- " + ex.Message, ex);
                return false;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("Master/SendMail/Exception:- " + ex.Message, ex);
                return false;
            }
        }

    }
}
