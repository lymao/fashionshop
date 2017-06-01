using AutoMapper;
using BotDetect.Web.Mvc;
using Common;
using Model.Models;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Infrastructure.Extensions;
using Web.Models;

namespace Web.Controllers
{
    public class ContactDetailController : Controller
    {
        IContactDetailService _contactDetailService;
        IFeedbackService _feedbackService;
        public ContactDetailController(IFeedbackService feedbackService,IContactDetailService contactDetailService)
        {
            this._contactDetailService = contactDetailService;
            this._feedbackService = feedbackService;
        }
        // GET: ContactDetail
        public ActionResult Index()
        {
            var newFeedbackViewModel = new FeedbackViewModel();
            newFeedbackViewModel.ContactDetail = GetContact();
            return View(newFeedbackViewModel);
        }

        private ContactDetailViewModel GetContact()
        {
            var contact = _contactDetailService.GetDefaultContact();
            var contactViewModel = Mapper.Map<ContactDetail, ContactDetailViewModel>(contact);
            return contactViewModel;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CaptchaValidation("CaptchaCode", "ExampleCaptcha", "Mã Captcha không đúng!")]
        public ActionResult SenFeedback(FeedbackViewModel feedbackVm)
        {
            if (ModelState.IsValid)
            {
                var newFeedback = new Feedback();
                newFeedback.UpdateFeedback(feedbackVm);
                _feedbackService.Create(newFeedback);
                _feedbackService.Save();

                ViewData["SuccessMsg"] = "Gửi phản hồi thành công.";

                var content = System.IO.File.ReadAllText(Server.MapPath("/Assets/client/templates/contact_template.html"));
                content = content.Replace("{{Name}}", feedbackVm.Name);
                content = content.Replace("{{Email}}", feedbackVm.Email);
                content = content.Replace("{{Message}}", feedbackVm.Message);

                var adminEmail = ConfigHelper.GetByKey("AdminEmail");
                MailHelper.SendMail(adminEmail, "Thông tin liên hệ từ website Fashionshop", content);
            }
            
            feedbackVm.ContactDetail = GetContact();
            return View("Index",feedbackVm);
        }
    }
}