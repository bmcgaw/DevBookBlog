﻿using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DevBook.Models
{
    public static class RoleInitializer
    {
       
        public static async Task InitializeRoles(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "Moderator","Guest","Subscriber" };

            IdentityResult roleResult;

           foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(role));               
                }
            
            }


            var adminEmail = configuration["AdminEmail"];
            var adminPassword = configuration["AdminPassword"];

            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail
            };

            var user = await userManager.FindByEmailAsync(adminEmail);

            if (user == null)
            {
                var createAdminUser = await userManager.CreateAsync(adminUser, adminPassword);
                if (createAdminUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
          
            }
       
        }

    }

}
