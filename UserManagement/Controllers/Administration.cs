using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.ViewModels;

namespace UserManagement.Controllers
{
   [Authorize(Roles ="Admin")]
    public class Administration : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public Administration(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._context = context;
        }
        //user management
        [HttpGet]
        public ViewResult ListUsers()
        {
            return View(_userManager.Users);
        }
        [HttpGet]
        public async Task<RedirectToActionResult> DeleteUser([FromRoute] string id)
        {

            if (id != null)
            {
                var user = await _userManager.FindByIdAsync(id);
                _context.Entry(user).Reference(user => user.infoUser).Load();
                if (user != null)
                {
                    if (user.infoUser != null)
                    {
                        _context.InfoUsers.Remove(user.infoUser);
                        await _context.SaveChangesAsync();
                    }
                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListUsers");
                    }

                }
            }

            return RedirectToAction("ListUsers");
        }
        [HttpGet]
        public async Task<IActionResult> EditUser([FromRoute] string id)
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
            return RedirectToAction("ListUsers");
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel edit)
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
                    return RedirectToAction("ListUsers");
                }
            }
            return RedirectToAction("ListUsers");
        }

        [HttpGet]
        public async Task<IActionResult> ApprovedUser(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return RedirectToAction("ListUsers");
                }
                else
                {
                    user.Approved = true;
                    await _userManager.UpdateAsync(user);
                    return RedirectToAction("ListUsers");
                }
            }
            return RedirectToAction("ListUsers");
        }

        //role management
        [HttpGet]
        public IActionResult ListRoles()
        {
            return View(_roleManager.Roles);
        }
        [HttpGet]
        public async Task<IActionResult> EditRole([FromRoute] string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    EditRoleViewModel editRole = new EditRoleViewModel()
                    {
                        ID = role.Id,
                        Name = role.Name
                    };
                    return View(editRole);

                }
            }

            return View("ListRoles");
        }
        [HttpPost]
        public async Task<RedirectToActionResult> EditRole(EditRoleViewModel editRole)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(editRole.ID);
                if (role != null)
                {
                    role.Name = editRole.Name;
                    role.NormalizedName = editRole.Name.ToUpper();
                    await _roleManager.UpdateAsync(role);
                    return RedirectToAction("ListRoles");
                }
            }
            return RedirectToAction("ListRoles");
        }
        [HttpGet]
        public async Task<RedirectToActionResult> DeleteRole([FromRoute] string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    await _roleManager.DeleteAsync(role);
                    return RedirectToAction("ListRoles");
                }
            }
            return RedirectToAction("ListRoles");
        }
        [HttpGet]
        public IActionResult AddANewRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddANewRole(AddRoleViewModel addRole)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByNameAsync(addRole.RoleName);
                if(role == null)
                {
                    var newRole = new IdentityRole() { Name = addRole.RoleName };
                    var result = await _roleManager.CreateAsync(newRole);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return RedirectToAction("ListRoles");
        }
        [HttpGet]
        public IActionResult AddUserToRole()
        {
            return View();
        }
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsRoleNameExists(string RoleName)
        {
            var role = await _roleManager.FindByNameAsync(RoleName);
            if (role != null)
            {
                return Json(true);
            }
            return Json($"Role name {RoleName} is not exists");
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToRole(AddUserToRoleViewModel addUserToRole)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(addUserToRole.Email);
                if (user != null)
                {
                    var result = await _userManager.AddToRoleAsync(user, addUserToRole.RoleName);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }
                }
            }
            return RedirectToAction("ListRoles");
        }
    }
}
