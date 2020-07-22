using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Data;
using UserManagement.Data.Interface;
using UserManagement.Models;
using UserManagement.ViewModels;

namespace UserManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IMailer _mailer { get; }

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IMailer mailer)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._mailer = mailer;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsEmailInUse(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return Json(true);
            }
            return Json($"Email {Email} is already in use");
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    UserName = register.Email,
                    Email = register.Email,
                    Approved = false
                };
                var result =  await _userManager.CreateAsync(user, register.Password);
                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token = token }, Request.Scheme);
                    await _mailer.SendEmail(confirmLink, user.Email, "Your account is Succesfully created"
                        , "Please check this Link to confirmation your account : ");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("Error");
                }
            }

            return View("ConfirmEmail");
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(userId == null || token == null)
            {
                return RedirectToAction("Register", "Account");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if(user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ViewBag.ErrorMessage = "cannot confirm your email";
                    return View("Error");
                }
            }
            return View();
        }

       
        [HttpGet]
        public IActionResult Login(string ReturnUrl)
        {
            LoginViewModel loginModel = new LoginViewModel()
            {
                returnUrl = ReturnUrl
            };
            return View(loginModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginModel.Email);
                if(user != null && user.EmailConfirmed == false && (await _userManager.CheckPasswordAsync(user, loginModel.Password)) )
                {
                    ModelState.AddModelError("", "Email not confirmed yet");
                    return View();
                }else if(user != null && user.EmailConfirmed == true && (await _userManager.CheckPasswordAsync(user, loginModel.Password) && user.Approved == false))
                {
                    ModelState.AddModelError("", "Your account not Approval yet");
                    return View();
                }else if(!await _userManager.CheckPasswordAsync(user, loginModel.Password)){
                    ModelState.AddModelError("", "Your password is not true");
                    return View();
                }
                var result = await _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl))
                    {
                        return LocalRedirect(ReturnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View("Error");
                }
            }
           
            return View();
        }
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> CheckEmailUnused(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                return Json(true);
            }
            return Json($"Email {Email} unused to resgister, Please register fisrt to login");
        }
        [HttpGet]
        public async Task<RedirectToActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        } 
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgotModel.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Email not exists !");
                    return View();
                }
                else if (! await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError("", "Email not confirmed yet !");
                    return View();
                }
                else
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = user.Email, token = token }, Request.Scheme);
                    await _mailer.SendEmail(passwordResetLink, user.Email, "Reset Password", "Please click this link to comfirmation to " +
                        "Reset your password: ");
                    return View("ConfirmEmail");
                }
                
            }
            return View();
        }
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            else
            {
                ResetPasswordViewModel reset = new ResetPasswordViewModel()
                {
                    Email = email,
                    Token = token
                };
                return View(reset);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel reset)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(reset.Email);
                if(user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, reset.Token, reset.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            return View("Login");
        }
    }
}
