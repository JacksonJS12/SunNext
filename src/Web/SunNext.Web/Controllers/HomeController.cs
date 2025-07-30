using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SunNext.Web.ViewModels.Home;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace SunNext.Web.Controllers
{
    using System.Diagnostics;
    using SunNext.Web.ViewModels;
    using Microsoft.AspNetCore.Mvc;

    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return this.View(new ContactFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await SendEmailAsync(model);
                TempData["SuccessMessage"] = "Your message has been sent successfully! We'll get back to you soon.";
                return RedirectToAction("Contact");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "There was an error sending your message. Please try again later.");
                return View(model);
            }
        }

        private async Task SendEmailAsync(ContactFormViewModel model)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            
            using var client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"]))
            {
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = bool.Parse(smtpSettings["EnableSsl"])
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["FromEmail"], smtpSettings["FromName"]),
                Subject = $"Contact Form Message from {model.Name}",
                Body = $@"
                    <h3>New Contact Form Submission</h3>
                    <p><strong>Name:</strong> {model.Name}</p>
                    <p><strong>Email:</strong> {model.Email}</p>
                    <p><strong>Message:</strong></p>
                    <p>{model.Message.Replace("\n", "<br>")}</p>
                    <hr>
                        <p><small>Sent from SunNext website contact form</small></p>
                ",
                IsBodyHtml = true
            };

            mailMessage.To.Add("d.ivanov@lumen101.com");
            mailMessage.ReplyToList.Add(new MailAddress(model.Email, model.Name));

            await client.SendMailAsync(mailMessage);
        }

        public IActionResult About()
        {
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [Route("Error/404")]
        public IActionResult Error404() => View("Error404");

        [Route("Error/500")]
        public IActionResult Error500()
        {
            return View("Error500");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        public IActionResult TriggerError()
        {
            throw new Exception("Simulated internal server error.");
        }
    }
}