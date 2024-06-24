using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DevBook.Models
{
    public static class RoleInitializer
    {
       
        public static async Task InitializeRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "Visitor" };

            IdentityResult roleResult;

           foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(role));                }
                {
                    
                }
            }

            var adminUser = new IdentityUser
            {
                UserName = "brian@admin.com",
                Email = "brian@admin.com"
            };

            var adminPassword = "Admin123!";

            var user = await userManager.FindByEmailAsync("brian@admin.com");

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
