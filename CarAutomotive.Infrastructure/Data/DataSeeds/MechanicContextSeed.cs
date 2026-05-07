namespace CarAutomotive.Infrastructure.Data.DataSeeds
{
    public class MechanicContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            var oldMechanics = await context.MechanicProfiles.ToListAsync();

            if (oldMechanics.Any())
            {
                context.MechanicProfiles.RemoveRange(oldMechanics);
                await context.SaveChangesAsync();
            }

            var user = await userManager.FindByEmailAsync("muhamed@gg.com");

            if (user == null)
            {
                throw new InvalidOperationException("Seeding Failed: The user 'muhamed@gg.com' was not found in the database!");
            }

            var mechanics = new List<MechanicProfile>
            {
                new MechanicProfile
                {
                    UserId = user.Id,
                    Name = "AutoFix 10th of Ramadan",
                    PhoneNumber = "01011111111",
                    Address = "10th of Ramadan City, Industrial Zone",
                    YearsOfExperience = 12,
                    Location = new Point(31.7420, 30.3060) { SRID = 4326 }
                },
                new MechanicProfile
                {
                    UserId = user.Id,
                    Name = "Cairo Stars Mechanics",
                    PhoneNumber = "01022222222",
                    Address = "Cairo, Nasr City, Makram Ebeid St",
                    YearsOfExperience = 15,
                    Location = new Point(31.3360, 30.0620) { SRID = 4326 }
                }
            };

            context.MechanicProfiles.AddRange(mechanics);
            await context.SaveChangesAsync();
        }
    }
}