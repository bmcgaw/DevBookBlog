using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DevBook.Models
    {
        public static class RoleInitializer
        {
            public static async Task Initialize(IServiceProvider serviceProvider, IConfiguration configuration)
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                
                string[] roles = { "Admin", "Guest" };

                
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                
                var adminEmail = configuration["Admin:Email"];
                var adminPassword = configuration["Admin:Password"];
                var adminFirstName = configuration["Admin:FirstName"];
                var adminLastName = configuration["Admin:LastName"];

                Console.WriteLine($"Admin Email: {adminEmail}");
                Console.WriteLine($"Admin Password: {adminPassword}");
                Console.WriteLine($"Admin First Name: {adminFirstName}");
                Console.WriteLine($"Admin Last Name: {adminLastName}");

            if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = adminFirstName,
                        LastName = adminLastName
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }
        }
    }


