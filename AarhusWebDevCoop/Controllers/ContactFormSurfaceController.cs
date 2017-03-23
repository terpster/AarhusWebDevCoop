using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AarhusWebDevCoop.ViewModels;
using System.Net.Mail;
using Umbraco.Web.Mvc;
using Umbraco.Core.Models;

namespace AarhusWebDevCoop.Controllers
{
    public class ContactFormSurfaceController : SurfaceController
    {
        // GET: ContactFormSurface
        public ActionResult Index()
        {
            return PartialView("ContactForm", new ContactForm());
        }

        [HttpPost]
        public ActionResult HandleFormSubmit(ContactForm model)
        {
            if (!ModelState.IsValid) { return CurrentUmbracoPage(); }

            else
            {
                MailMessage message = new MailMessage();
                message.To.Add("evanbreener@gmail.com");
                message.Subject = model.Subject;
                message.From = new MailAddress(model.Email, model.Name);
                message.Body = model.Message;

                IContent comment = Services.ContentService.CreateContent(model.Subject, CurrentPage.Id, "Message");
                comment.SetValue("messageName", model.Name);
                comment.SetValue("email", model.Email);
                comment.SetValue("subject", model.Subject);
                comment.SetValue("messageContent", model.Message);

                Services.ContentService.Save(comment);

                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.EnableSsl = true;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential("evanbreener@gmail.com", "evanbreen1234");
                    smtp.EnableSsl = true;

                    smtp.Send(message);
                }

                TempData["success"] = true;

                return RedirectToCurrentUmbracoPage();
            }
        }
    }
}