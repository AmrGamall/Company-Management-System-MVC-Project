using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INewEmailSettings _newEmailSettings;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager, 
            INewEmailSettings newEmailSettings)
        {
            _userManager = userManager;
			_signInManager = signInManager;
            _newEmailSettings = newEmailSettings;
        }

		#region Register

		[HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var User = new ApplicationUser()
                {
                    UserName = model.Email.Split('@')[0],
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IsAgree = model.IsAgree,
                };
                var Result = await _userManager.CreateAsync(User, model.Password);
                if (Result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (var error in Result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        #endregion


        #region Login

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    var Result = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (Result)
                    {
                        var LoginResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                        if (LoginResult.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Password is Incorrect");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email is not Exists");
                }
            }
            return View(model);
        }
        #endregion



        #region Sign Out

        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        #endregion


        #region Forget Password
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> SendEmail(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByEmailAsync(model.Email);
                if (User is not null)
                {

                    var token = await _userManager.GeneratePasswordResetTokenAsync(User);

                    var ResetPasswordLink = Url.Action("ResetPassword", "Account", new { email = User.Email, Token = token }, Request.Scheme);

                    var email = new NewEmail()
                    {
                        To = model.Email,
                        Subject = "Reset Password",
                        Body = ResetPasswordLink
                    };

                    //send Email

                    //NewEmailSettings emailSettings = new NewEmailSettings();

                    _newEmailSettings.SendEmail(email);


                    return RedirectToAction(nameof(CheckYourInbox));

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email is not Exitsts");
                }

            }
             return View("ForgetPassword", model);

        }


        public IActionResult CheckYourInbox()
        {
            return View();
        }

        #endregion


        #region Reset Password
        public IActionResult ResetPassword(string email, string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string email = TempData["email"] as string;
                string token = TempData["token"] as string;
                var User = await _userManager.FindByEmailAsync(email);
                var Result = await _userManager.ResetPasswordAsync(User, token, model.NewPassword);

                if (Result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (var error in Result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);

                    }
                }

            }
            return View(model);

        }
        #endregion


        #region External Login

        public IActionResult GoogleLogin()
        {
            var prop = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(prop, GoogleDefaults.AuthenticationScheme);
        }



        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            var Claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            return RedirectToAction("Index", "Home");
        }

        #endregion



    }
}
