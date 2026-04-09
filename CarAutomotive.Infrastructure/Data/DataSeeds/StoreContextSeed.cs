namespace CarAutomotive.Infrastructure.Data.DataSeeds
{
    public class StoreContextSeed
    {
        public class AppIdentityDbContextSeed
        {
            public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
            {
                if (!userManager.Users.Any())
                {
                    var user = new AppUser
                    {
                        FirstName = "Muhamed",      
                        LastName = "Husseny",
                        DisplayName = "Muhamed Husseny",
                        Email = "muhamed@gg.com",
                        UserName = "muhamed_husseny",
                        PhoneNumber = "01012345678"
                    };

                    await userManager.CreateAsync(user, "Pa$$w0rd123");
                }
            }
        }
    }
}
