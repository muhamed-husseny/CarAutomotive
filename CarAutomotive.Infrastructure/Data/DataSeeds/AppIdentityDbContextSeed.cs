namespace CarAutomotive.Infrastructure.Data.DataSeeds 
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedAdminUserAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("User"));
            }

            if (await userManager.FindByEmailAsync("admin2@carautomotive.com") == null)
            {
                var adminUser = new AppUser
                {
                    DisplayName = "Super Admin",
                    FirstName = "Super",
                    LastName = "Admin",
                    Email = "admin2@carautomotive.com",
                    UserName = "admin2"
                };

                var result = await userManager.CreateAsync(adminUser, "AdminPassword123!@#");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            
        }
    }
}