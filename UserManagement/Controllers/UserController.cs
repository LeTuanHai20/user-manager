using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.ViewModels;

namespace UserManagement.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public UserController(UserManager<User> userManager, ApplicationDbContext context)
        {
            this._userManager = userManager;
            this._context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            ViewBag.userId = _userManager.GetUserId(HttpContext.User);
            var id =  _userManager.GetUserId(HttpContext.User);
            if(id != null)
            {
                var user = await _userManager.FindByIdAsync(id);
                _context.Entry(user).Reference(user => user.infoUser).Load();
                if(user != null)
                {
                    if (user.infoUser == null)
                    {
                        user.infoUser = new InfoUser();
                        ProfileViewModel profile = new ProfileViewModel()
                        {
                            infoId = user.infoUser.InfoId,
                            Email = user.Email,
                            Enable = user.infoUser.Enable,
                            FullName = user.infoUser.FullName
                        };
                        return View(profile);
                    }
                    else
                    {
                        ProfileViewModel profile = new ProfileViewModel()
                        {
                            infoId = user.infoUser.InfoId,
                            Email = user.Email,
                            Enable = user.infoUser.Enable,
                            FullName = user.infoUser.FullName
                        };
                        return View(profile);
                    }
                }
            }
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> EditProfile([FromRoute] string id)
        {
            if (id != null)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    _context.Entry(user).Reference(user => user.infoUser).Load();
                    if (user.infoUser == null)
                    {
                        EditUserViewModel edit = new EditUserViewModel()
                        {
                            ID = user.Id,
                            Email = user.Email,
                        };
                        return View(edit);
                    }
                    else
                    {
                        EditUserViewModel edit = new EditUserViewModel()
                        {
                            ID = user.Id,
                            Email = user.Email,
                            FullName = user.infoUser.FullName,
                            Enable = user.infoUser.Enable.ToString()
                        };
                        return View(edit);
                    }

                }
            }
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditUserViewModel edit)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(edit.ID);
                if (user != null)
                {
                    user.Email = edit.Email;
                    _context.Entry(user).Reference(user => user.infoUser).Load();

                    if (user.infoUser == null)
                    {
                        user.infoUser = new InfoUser();
                        user.infoUser.FullName = edit.FullName;
                        if (edit.Enable == "true")
                        {
                            user.infoUser.Enable = true;
                        }
                        else if (edit.Enable == "false")
                        {
                            user.infoUser.Enable = false;
                        }
                    }
                    else
                    {
                        user.infoUser.FullName = edit.FullName;
                        if (edit.Enable == "true")
                        {
                            user.infoUser.Enable = true;
                        }
                        else if (edit.Enable == "false")
                        {

                            user.infoUser.Enable = false;
                        }
                    }
                    await _userManager.UpdateAsync(user);
                    return RedirectToAction("Profile");
                }
            }
            return RedirectToAction("Profile");
        }
        public async Task<IActionResult> ConfirmOldPassword(string id)
        {
            if(id != null)
            {
                var user = await _userManager.FindByIdAsync(id);
                if(user != null)
                {
                    ConfirmOldPassword confirmPassword = new ConfirmOldPassword()
                    {
                        UserId = user.Id
                    };
                    return View(confirmPassword);
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmOldPassword(ConfirmOldPassword confirmPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(confirmPassword.UserId);
                if(user != null)
                {
                    var result = await _userManager.CheckPasswordAsync(user, confirmPassword.OldPassword);
                    if (!result)
                    {
                        ModelState.AddModelError("", "Your password is not true");
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("ChangePassword", new {Id = user.Id , OldPassword = confirmPassword.OldPassword});
                    }
                }
            }
            return RedirectToAction("ConfirmOldPassword");
        }
        [HttpGet]
        public ViewResult ChangePassword(string Id, string OldPassword)
        {
            ChangePasswordViewModel changePassword = new ChangePasswordViewModel()
            {
                UserId = Id,
                OldPassword = OldPassword
            };
            return View(changePassword);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(changePassword.UserId);
                if(user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        ModelState.AddModelError("","Please try agian !");
                        return View();
                    }
                }
            }
            return RedirectToAction("Profile");
        }

    }
}
