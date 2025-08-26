using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace MediaLibraryApp
{
    public static class DataSeeder
    {
        public static async Task SeedDemoUserAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string demoEmail = "test@test.com";
            string demoPassword = "Pass_1234"; // pick something simple but valid

            var existingUser = await userManager.FindByEmailAsync(demoEmail);
            if (existingUser == null)
            {
                var demoUser = new IdentityUser
                {
                    UserName = demoEmail,
                    Email = demoEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(demoUser, demoPassword);
            }
        }
    }
}