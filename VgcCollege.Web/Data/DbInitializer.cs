using Microsoft.AspNetCore.Identity;
using VgcCollege.Domain;

namespace VgcCollege.Web.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 1. Create Roles
            string[] roleNames = { "Admin", "Faculty", "Student" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Seed Admin User
            if (await userManager.FindByEmailAsync("admin@vgc.ie") == null)
            {
                var admin = new IdentityUser { UserName = "admin@vgc.ie", Email = "admin@vgc.ie", EmailConfirmed = true };
                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // 3. Seed Faculty User
            if (await userManager.FindByEmailAsync("teacher@vgc.ie") == null)
            {
                var facultyUser = new IdentityUser { UserName = "teacher@vgc.ie", Email = "teacher@vgc.ie", EmailConfirmed = true };
                await userManager.CreateAsync(facultyUser, "Faculty123!");
                await userManager.AddToRoleAsync(facultyUser, "Faculty");

                // Create the profile in the Domain
                context.FacultyProfiles.Add(new FacultyProfile
                {
                    IdentityUserId = facultyUser.Id,
                    Name = "Professor X",
                    Email = facultyUser.Email
                });
            }

            // 4. Seed Student Users
            if (await userManager.FindByEmailAsync("student1@vgc.ie") == null)
            {
                var studentUser = new IdentityUser { UserName = "student1@vgc.ie", Email = "student1@vgc.ie", EmailConfirmed = true };
                await userManager.CreateAsync(studentUser, "Student123!");
                await userManager.AddToRoleAsync(studentUser, "Student");

                context.StudentProfiles.Add(new StudentProfile
                {
                    IdentityUserId = studentUser.Id,
                    Name = "John Doe",
                    Email = studentUser.Email,
                    StudentNumber = "VGC1001"
                });
            }

            await context.SaveChangesAsync();
        }
    }
}