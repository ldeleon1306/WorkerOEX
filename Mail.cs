using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using Workers.Models;

namespace Workers.Mail
{
    public class Mail
    {
        private readonly IConfiguration _configuration;
        private string _mailTo { get; set; }
        private string _mailFrom { get; set; }
        private string _smtpServer { get; set; }

        public Mail(IConfiguration configuration)
        {
            _configuration = configuration;
            _mailTo = _configuration["MailSmtp:mailTo"];
            _mailFrom = _configuration["MailSmtp:mailFrom"];
            _smtpServer = _configuration["MailSmtp:smtp"];
        }

        public void SendEmail(List<WAP_INGRESOPEDIDOS> listWapReprocesar, string cliente)
        {
            try
            {
                Console.WriteLine("entrando al mail");
                MailMessage mail = new MailMessage();
                //Console.WriteLine(_mailFrom.ToString());
                //Console.WriteLine(_mailTo);
                mail.From = new MailAddress("appdesabrdcsrv@andreani.com");
                mail.To.Add("ldeleon@andreani.com");
                //var multiple = _mailTo.Split(';');
                //foreach (var to in multiple)
                //{
                //    if (to != string.Empty)
                //        mail.To.Add(to);
                //}              
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment("output.txt");
                mail.Attachments.Add(attachment);
                //if (!String.IsNullOrEmpty(txterror))
                //{
                //  attachment = new System.Net.Mail.Attachment(txterror);
                //  mail.Attachments.Add(attachment);
                //}

                string subject = string.Format($"Prueba {cliente} Mail");
                string bodyMsg = string.Format($"Se procesó un mongo {cliente}");
                mail.Subject = subject;
                mail.Body = bodyMsg;
                mail.IsBodyHtml = true;
                Console.WriteLine(bodyMsg);
                SmtpClient smtp = new SmtpClient("10.20.7.16");
                smtp.EnableSsl = false;
                smtp.Port = 25;
                smtp.UseDefaultCredentials = true;

                //string user = "leosendmailoe@gmail.com";
                //string pass = "ordenexterna";
                //NetworkCredential userCredential = new NetworkCredential(user, pass);

                //smtp.Credentials = userCredential;
                Console.WriteLine("antes  smtp.Send(mail)");
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("dentro de casa mail");
                //Console.WriteLine(_mailFrom);
                //Console.WriteLine(_mailTo);
                Console.WriteLine(ex.Message);
                //_logger.LogError(ex, "Se produjo una excepción en el metodo SendEmail: ", ex.Message);
            }
        }
    }
}
