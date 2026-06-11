using CarAutomotive.Core.Entities;
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
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!await context.Categories.AnyAsync())
            {
                var categoriesData = await File.ReadAllTextAsync(
                    "../CarAutomotive.Infrastructure/Data/DataSeeds/SeedData/categories.json");

                var categories = JsonSerializer.Deserialize<List<Category>>(
                    categoriesData,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (categories != null && categories.Count > 0)
                {
                    await context.Categories.AddRangeAsync(categories);
                    await context.SaveChangesAsync();
                }
            }

            if (!await context.Products.AnyAsync())
            {
                var productsData = await File.ReadAllTextAsync(
                    "../CarAutomotive.Infrastructure/Data/DataSeeds/SeedData/products.json");

                var products = JsonSerializer.Deserialize<List<Product>>(
                    productsData,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (products != null && products.Count > 0)
                {
                    await context.Products.AddRangeAsync(products);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
