using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using UserManagement.Models;

namespace UserManagement.Data
{
    public static class IdentityDataInitializer
    {
        public static void SeedData(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Admin"
                };
                roleManager.CreateAsync(role).Wait();
            }

           
        }

        private static void SeedUsers(UserManager<User> userManager)
        {
            if (userManager.FindByNameAsync("tuanhai@gmail.com").Result == null)
            {
                var user = new User
                {
                    UserName = "tuanhai@gmail.com",
                    Email = "tuanhai@gmail.com",
                    Approved = true,
                    EmailConfirmed = true
                };

                IdentityResult result = userManager.CreateAsync(user, "hai123").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }
    }
}
