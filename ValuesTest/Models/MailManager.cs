using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Configuration;
using System.Net.Mail;
using DALA;
using System.Configuration;
using System.Net;

namespace Web.Models
{
    public class MailManager
    {
        static public void InformUser(User user,OrderDetailDTO orderDetailDTO)
        {
            SmtpSection secObj = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            var productsmessage = "";

            foreach (var product in orderDetailDTO.Products)
            {
                productsmessage +=
                    $"<div>" +
                    $"<span>{product.name} x</span></span>{product.quantity}</span>" +
                    $"</div>";
            }

            var message =
                $"<h1 style='color:teal'>{user.Name} Sit tight Your Order #{orderDetailDTO.Order_Id} was placed!</h1>" +
                $"{productsmessage}" +
                $"<div>Total: {orderDetailDTO.Amount}</div>";
            var mailmessage = new MailMessage();
            mailmessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailmessage.IsBodyHtml = true;
            mailmessage.To.Add($"{user.Email}");
            mailmessage.From = new MailAddress(secObj.From);
            mailmessage.Subject = "Thank You For Ordering :)";
            mailmessage.Body = message;

            var smtp = new SmtpClient();
            smtp.Host = secObj.Network.Host; //---- SMTP Host Details. 
            smtp.EnableSsl = secObj.Network.EnableSsl; //---- Specify whether host accepts SSL Connections or not.
            NetworkCredential NetworkCred = new NetworkCredential(secObj.Network.UserName, secObj.Network.Password);
            //---Your Email and password
            
            smtp.Credentials = NetworkCred;
            smtp.Port = 587; //---- SMTP Server port number. This varies from host to host. 
            

            try
            {
                smtp.Send(mailmessage);
            }
            catch (Exception)
            {

                throw new SmtpFailedRecipientException();
            }
           
           
        }
    }
}