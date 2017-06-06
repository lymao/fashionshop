using AutoMapper;
using BotDetect.Web.Mvc;
using Common;
using Common.Exceptions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Infrastructure.Extensions;
using Web.Models;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Account
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var model = _userManager.FindById(userId);
            var modelViewModel = Mapper.Map<ApplicationUser, ApplicationUserViewModel>(model);
            return View(modelViewModel);
        }

        public ActionResult ChangeAccount()
        {
            var userId = User.Identity.GetUserId();
            var model = _userManager.FindById(userId);
            var modelViewModel = Mapper.Map<ApplicationUser, ApplicationUserViewModel>(model);
            return View(modelViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeAccount(ApplicationUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                try
                {
                    var checkmail = await _userManager.FindByEmailAsync(model.Email);
                    if (checkmail != null)
                    {
                        if (checkmail.Id != model.Id)
                        {
                            throw new NameDuplicatedException("Email này đã được sử dụng.");
                        }
                    }
                    user.UpdateUser(model);
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        ViewData["success"] = "Cập nhật thành công.";
                    }
                    else
                    {
                        ModelState.AddModelError("", string.Join(",", result.Errors));
                    }

                }
                catch (NameDuplicatedException dex)
                {
                    ModelState.AddModelError("", dex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var userId = User.Identity.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            var userCheck = _userManager.Find(user.UserName, model.OldPassword);
            if (userCheck == null)
            {
                ModelState.AddModelError("", "Mật khẩu của bạn không đúng.");
                return View();
            }
            var result = await _userManager.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
                authenticationManager.SignOut();
                TempData["success"] = "Mật khẩu đã được thay đổi";
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", string.Join(",", result.Errors));
            }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CaptchaValidation("CaptchaCode", "ExampleCaptcha", "Mã Captcha không đúng!")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userByEmail = await _userManager.FindByEmailAsync(model.Email);
                if (userByEmail != null)
                {
                    ModelState.AddModelError("email", "Email này đã tồn tại.");
                    return View(model);
                }
                var userByUserName = await _userManager.FindByNameAsync(model.UserName);
                if (userByUserName != null)
                {
                    ModelState.AddModelError("userName", "Tên tài khoản này đã tồn tại.");
                    return View(model);
                }
                var user = new ApplicationUser()
                {
                    FullName = model.FullName,
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed = true,
                    BirthDay = DateTime.Now,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber
                };
                await _userManager.CreateAsync(user, model.Password);
                var adminUser = await _userManager.FindByEmailAsync(model.Email);
                if (adminUser != null)
                    await _userManager.AddToRolesAsync(adminUser.Id, new string[] { "User" });

                string content = System.IO.File.ReadAllText(Server.MapPath("/Assets/client/templates/newuser.html"));
                content = content.Replace("{{UserName}}", adminUser.UserName);
                content = content.Replace("{{PassWord}}", model.Password);
                content = content.Replace("{{Link}}", ConfigHelper.GetByKey("CurrentLink") + "dang-nhap.html");

                MailHelper.SendMail(adminUser.Email, "Đăng ký thành công.", content);

                ViewData["SuccessMsg"] = "đăng ký thành công.";
            }
            return View();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)

        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
                    authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                    ClaimsIdentity identity = _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationProperties props = new AuthenticationProperties();
                    props.IsPersistent = model.RememberMe;
                    authenticationManager.SignIn(props, identity);
                    //if (Url.IsLocalUrl(returnUrl))
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            ControllerContext.HttpContext.Session.RemoveAll();
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

    }
}