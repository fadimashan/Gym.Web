using Bogus;
using Gym.Core.Entities;
using Gym.Data.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gym.Data
{
    public class SeedData
    {
        public static async Task InitAsync(IServiceProvider services, string adminPW)
        {
            using (var context = new ApplicationDbContext(services.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {

                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                var faker = new Faker();

                var gymclasses = new List<GymClass>();

                for (int i = 0; i < 20; i++)
                {
                    var gymClass = new GymClass
                    {
                        Name = faker.Company.CatchPhrase(),
                        Discription = faker.Hacker.Verb(),
                        Duration = new TimeSpan(0, 55, 0),
                        StartDate = DateTime.Now.AddDays(faker.Random.Int(-2, 2))
                    };

                    gymclasses.Add(gymClass);
                }

                await context.AddRangeAsync(gymclasses);

                var roleNames = new[] { "Admin", "Member" };


                foreach (var roleName in roleNames)
                {
                    if (await roleManager.RoleExistsAsync(roleName)) continue;

                    var role = new IdentityRole { Name = roleName };
                    var result = await roleManager.CreateAsync(role);

                    if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
                }

                var adminEmail ="admin@gym.com";
                var adminName = "Admin";
                var foundAdmin = await userManager.FindByEmailAsync(adminEmail);

                if (foundAdmin != null) return;

                var admin = new ApplicationUser
                {
                    UserName = adminName,
                    Email = adminEmail,
                    PersonalIdNum = "1234-12-12-1234",
                    Birthdate = DateTime.Now - TimeSpan.FromDays(10000)

                };

                var addAdminResult = await userManager.CreateAsync(admin, adminPW);

                if (!addAdminResult.Succeeded) throw new Exception(string.Join("\n", addAdminResult.Errors));

                var adminUser = await userManager.FindByNameAsync(adminName);

                foreach (var role in roleNames)
                {
                    if (await userManager.IsInRoleAsync(adminUser,role)) continue;

                    var addRoRoleResult = await userManager.AddToRoleAsync(adminUser, role);

                    if (!addRoRoleResult.Succeeded) throw new Exception(string.Join("\n", addRoRoleResult.Errors));

                }

                await context.SaveChangesAsync();
            }

        }
    }
}