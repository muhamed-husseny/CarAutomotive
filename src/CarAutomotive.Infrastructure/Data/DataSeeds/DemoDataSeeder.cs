using CarAutomotive.Core.Entities;
using CarAutomotive.Core.Entities.Identity;
using CarAutomotive.Core.Entities.Mechanic;
using CarAutomotive.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CarAutomotive.Infrastructure.Data.DataSeeds
{
    public static class DemoDataSeeder
    {
        public const string DefaultDemoPassword = "DemoPass123!";

        private static readonly string[] RequiredRoles = { "Admin", "User", "Mechanic", "Merchant" };

        public static async Task SeedAsync(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            string? demoPassword = null)
        {
            var password = string.IsNullOrWhiteSpace(demoPassword) ? DefaultDemoPassword : demoPassword;

            // 1. Seed Roles (idempotent)
            foreach (var roleName in RequiredRoles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }

            // 2. Seed 4 Flagship Demo Accounts (idempotent)
            var adminUser = await EnsureUserAsync(userManager, "admin@carautomotive.demo", "demo_admin", "Omar", "El-Sayed", "Admin", password);
            var customerUser = await EnsureUserAsync(userManager, "customer@carautomotive.demo", "demo_customer", "Youssef", "Hassan", "User", password);
            var mechanicUser = await EnsureUserAsync(userManager, "mechanic@carautomotive.demo", "demo_mechanic", "Tarek", "Mansour", "Mechanic", password);
            var merchantUser = await EnsureUserAsync(userManager, "merchant@carautomotive.demo", "demo_merchant", "Karim", "Adel", "Merchant", password);

            // Ensure Merchant Profile (idempotent)
            var existingMerchant = await context.Set<Merchants>().FirstOrDefaultAsync(m => m.AppUserId == merchantUser.Id);
            if (existingMerchant == null)
            {
                context.Set<Merchants>().Add(new Merchants
                {
                    AppUserId = merchantUser.Id,
                    ShopName = "Cairo Auto Parts Hub",
                    CommercialRegister = "CR-EG-2026-9041",
                    Status = "APPROVED"
                });
                await context.SaveChangesAsync();
            }

            // 3. Seed Customer Demo Vehicles (idempotent)
            await SeedCustomerVehiclesAsync(context, customerUser.Id);

            // 4. Seed 6 Cairo & Giza Workshops with real PostGIS coordinates (idempotent)
            await SeedCairoGizaWorkshopsAsync(context, userManager, mechanicUser, password);

            // 5. Seed Brands, Categories, Spare Parts & Vehicle Fitment Compatibilities (idempotent)
            await SeedCatalogAndFitmentAsync(context, merchantUser.Id);
        }

        private static async Task<AppUser> EnsureUserAsync(
            UserManager<AppUser> userManager,
            string email,
            string userName,
            string firstName,
            string lastName,
            string role,
            string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    DisplayName = $"{firstName} {lastName}",
                    Email = email,
                    UserName = userName,
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to seed demo user '{email}': {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }

            return user;
        }

        private static async Task SeedCustomerVehiclesAsync(ApplicationDbContext context, Guid customerUserId)
        {
            var demoVehicles = new[]
            {
                new Vehicle
                {
                    Make = "Toyota",
                    Model = "Corolla",
                    Year = 2021,
                    PlateCode = "س ص ع",
                    PlateNumber = "4821",
                    Mileage = 54000,
                    FuelType = "Petrol",
                    Color = "Silver",
                    Status = VehicleStatus.Perfect,
                    Vin = "JTDBR32E720054321",
                    AppUserId = customerUserId
                },
                new Vehicle
                {
                    Make = "Hyundai",
                    Model = "Elantra",
                    Year = 2022,
                    PlateCode = "م ن ه",
                    PlateNumber = "7390",
                    Mileage = 31500,
                    FuelType = "Petrol",
                    Color = "Midnight Blue",
                    Status = VehicleStatus.Perfect,
                    Vin = "KMHD84LF6NU123456",
                    AppUserId = customerUserId
                }
            };

            foreach (var v in demoVehicles)
            {
                var exists = await context.Vehicles.AnyAsync(x =>
                    x.AppUserId == customerUserId && x.PlateNumber == v.PlateNumber);
                if (!exists)
                {
                    context.Vehicles.Add(v);
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedCairoGizaWorkshopsAsync(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            AppUser primaryMechanicUser,
            string password)
        {
            var workshops = new[]
            {
                (
                    OwnerEmail: primaryMechanicUser.Email!,
                    UserName: primaryMechanicUser.UserName!,
                    FirstName: "Tarek",
                    LastName: "Mansour",
                    Name: "Maadi Precision Auto Care",
                    Phone: "01012345001",
                    Address: "Road 9, Maadi, Cairo",
                    Lng: 31.2597,
                    Lat: 29.9602,
                    Rating: 4.9,
                    Reviews: 84,
                    Experience: 14,
                    Services: new[]
                    {
                        ("Brake Pad & Disc Service", "Full front/rear brake overhaul with OEM parts", 650m),
                        ("Computer OBD-II Diagnostics", "Full ECU scan and fault reset", 300m),
                        ("Synthetic Oil & Filter Change", "5W-30 full synthetic oil + original filter", 950m)
                    }
                ),
                (
                    OwnerEmail: "workshop.nasrcity@carautomotive.demo",
                    UserName: "workshop_nasrcity",
                    FirstName: "Hossam",
                    LastName: "Nabil",
                    Name: "Nasr City Japanese & Korean Masters",
                    Phone: "01012345002",
                    Address: "Makram Ebeid St, Nasr City, Cairo",
                    Lng: 31.3360,
                    Lat: 30.0626,
                    Rating: 4.7,
                    Reviews: 62,
                    Experience: 12,
                    Services: new[]
                    {
                        ("Periodic 10,000 km Maintenance", "Oil, filters, fluid top-up, 40-point check", 1400m),
                        ("AC Compressor & Freon Recharge", "Leak test and R134a recharge", 850m)
                    }
                ),
                (
                    OwnerEmail: "workshop.dokki@carautomotive.demo",
                    UserName: "workshop_dokki",
                    FirstName: "Mahmoud",
                    LastName: "Gamal",
                    Name: "Dokki Rapid Diagnostics & Brakes",
                    Phone: "01012345003",
                    Address: "Tahrir St, Dokki, Giza",
                    Lng: 31.2089,
                    Lat: 30.0385,
                    Rating: 4.6,
                    Reviews: 51,
                    Experience: 9,
                    Services: new[]
                    {
                        ("Brake Fluid Flush", "DOT 4 hydraulic brake bleeding", 350m),
                        ("Battery & Alternator Check", "Load test and charging system repair", 250m)
                    }
                ),
                (
                    OwnerEmail: "workshop.zayed@carautomotive.demo",
                    UserName: "workshop_zayed",
                    FirstName: "Sherif",
                    LastName: "Kamel",
                    Name: "Zayed Elite Motorworks",
                    Phone: "01012345004",
                    Address: "Central Axis, Sheikh Zayed, Giza",
                    Lng: 30.9830,
                    Lat: 30.0218,
                    Rating: 4.8,
                    Reviews: 73,
                    Experience: 11,
                    Services: new[]
                    {
                        ("3D Wheel Alignment & Balancing", "Laser alignment for all four wheels", 450m),
                        ("Suspension & Shock Absorber Service", "Control arms, bushings, and shocks", 1200m)
                    }
                ),
                (
                    OwnerEmail: "workshop.october@carautomotive.demo",
                    UserName: "workshop_october",
                    FirstName: "Mostafa",
                    LastName: "Farouk",
                    Name: "October Pro Suspension & Tires",
                    Phone: "01012345005",
                    Address: "Al Hosary Square, 6th of October, Giza",
                    Lng: 30.9420,
                    Lat: 29.9697,
                    Rating: 4.5,
                    Reviews: 39,
                    Experience: 8,
                    Services: new[]
                    {
                        ("Tire Mounting & Nitrogen Fill", "4-tire mount, balance, and nitrogen", 400m),
                        ("Front Suspension Rebuild", "Tie rods, stabilizer links, and alignment", 1500m)
                    }
                ),
                (
                    OwnerEmail: "workshop.zamalek@carautomotive.demo",
                    UserName: "workshop_zamalek",
                    FirstName: "Wael",
                    LastName: "Rashed",
                    Name: "Zamalek Concierge Auto Service",
                    Phone: "01012345006",
                    Address: "26th of July Corridor, Zamalek, Cairo",
                    Lng: 31.2194,
                    Lat: 30.0609,
                    Rating: 4.8,
                    Reviews: 47,
                    Experience: 15,
                    Services: new[]
                    {
                        ("Express Oil & Inspection", "30-minute express service with multi-point report", 1100m),
                        ("Ignition & Spark Plug Tune-Up", "Iridium spark plugs and coil testing", 750m)
                    }
                )
            };

            foreach (var w in workshops)
            {
                var owner = w.OwnerEmail == primaryMechanicUser.Email
                    ? primaryMechanicUser
                    : await EnsureUserAsync(userManager, w.OwnerEmail, w.UserName, w.FirstName, w.LastName, "Mechanic", password);

                var exists = await context.MechanicProfiles.AnyAsync(m => m.UserId == owner.Id || m.Name == w.Name);
                if (!exists)
                {
                    context.MechanicProfiles.Add(new MechanicProfile
                    {
                        UserId = owner.Id,
                        Name = w.Name,
                        PhoneNumber = w.Phone,
                        Address = w.Address,
                        Location = new Point(w.Lng, w.Lat) { SRID = 4326 },
                        AverageRating = w.Rating,
                        TotalReviews = w.Reviews,
                        YearsOfExperience = w.Experience,
                        IsAvailable = true,
                        Services = w.Services.Select(s => new MechanicService
                        {
                            ServiceName = s.Item1,
                            Description = s.Item2,
                            EstimatedPrice = s.Item3
                        }).ToList()
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedCatalogAndFitmentAsync(ApplicationDbContext context, Guid merchantUserId)
        {
            var brandNames = new[] { "Bosch", "Brembo", "Mobil 1", "NGK" };
            foreach (var name in brandNames)
            {
                if (!await context.Brands.AnyAsync(b => b.Name == name))
                {
                    context.Brands.Add(new Brand { Name = name });
                }
            }

            var categoryDefinitions = new[]
            {
                ("Brakes", "Brake pads, rotors, and hydraulic brake fluids"),
                ("Engine & Oils", "Synthetic motor oils and engine filters"),
                ("Suspension", "Shock absorbers, control arms, and bushings"),
                ("Ignition & Electrical", "Spark plugs, batteries, and alternators")
            };

            foreach (var (name, desc) in categoryDefinitions)
            {
                if (!await context.Categories.AnyAsync(c => c.Name == name))
                {
                    context.Categories.Add(new Category { Name = name, Description = desc });
                }
            }

            await context.SaveChangesAsync();

            var brands = await context.Brands.ToDictionaryAsync(b => b.Name, b => b.Id);
            var categories = await context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);

            var products = new[]
            {
                (
                    Title: "Brembo Ceramic Front Brake Pads",
                    Description: "Low-dust ceramic front brake pads engineered for Cairo stop-and-go traffic.",
                    Price: 1650m,
                    Stock: 30,
                    Category: "Brakes",
                    Brand: "Brembo",
                    OuterDiameterMM: (decimal?)280m,
                    Compatibilities: new[] { ("Toyota", "Corolla", 2021), ("Toyota", "Corolla", 2022), ("Hyundai", "Elantra", 2022) }
                ),
                (
                    Title: "Brembo Ventilated Front Brake Rotor",
                    Description: "High-carbon ventilated front disc rotor for superior heat dissipation.",
                    Price: 2450m,
                    Stock: 18,
                    Category: "Brakes",
                    Brand: "Brembo",
                    OuterDiameterMM: (decimal?)282m,
                    Compatibilities: new[] { ("Toyota", "Corolla", 2021), ("Kia", "Cerato", 2021) }
                ),
                (
                    Title: "Mobil 1 Full Synthetic 5W-30 (4L)",
                    Description: "Advanced full synthetic engine oil providing up to 10,000 km protection.",
                    Price: 1350m,
                    Stock: 50,
                    Category: "Engine & Oils",
                    Brand: "Mobil 1",
                    OuterDiameterMM: (decimal?)null,
                    Compatibilities: new[] { ("Toyota", "Corolla", 2021), ("Hyundai", "Elantra", 2022), ("Nissan", "Sunny", 2022) }
                ),
                (
                    Title: "Bosch Premium Spin-On Oil Filter",
                    Description: "High-efficiency filtration media capturing 99% of engine contaminants.",
                    Price: 240m,
                    Stock: 75,
                    Category: "Engine & Oils",
                    Brand: "Bosch",
                    OuterDiameterMM: (decimal?)null,
                    Compatibilities: new[] { ("Toyota", "Corolla", 2021), ("Hyundai", "Elantra", 2022) }
                ),
                (
                    Title: "NGK Laser Iridium Spark Plugs (Set of 4)",
                    Description: "Long-life iridium tipped spark plugs for smoother idle and better fuel economy.",
                    Price: 980m,
                    Stock: 40,
                    Category: "Ignition & Electrical",
                    Brand: "NGK",
                    OuterDiameterMM: (decimal?)null,
                    Compatibilities: new[] { ("Toyota", "Corolla", 2021), ("Hyundai", "Elantra", 2022), ("Kia", "Cerato", 2021) }
                ),
                (
                    Title: "Bosch Front Gas Shock Absorber Pair",
                    Description: "Twin-tube gas-charged shocks built for comfort on Egyptian roads.",
                    Price: 3100m,
                    Stock: 14,
                    Category: "Suspension",
                    Brand: "Bosch",
                    OuterDiameterMM: (decimal?)null,
                    Compatibilities: new[] { ("Hyundai", "Elantra", 2022), ("Kia", "Cerato", 2021) }
                )
            };

            foreach (var p in products)
            {
                var product = await context.Products
                    .Include(x => x.Compatibilities)
                    .FirstOrDefaultAsync(x => x.Title == p.Title);

                if (product == null)
                {
                    product = new Product
                    {
                        Title = p.Title,
                        Description = p.Description,
                        BasePriceEgp = p.Price,
                        StockQuantity = p.Stock,
                        CreatedDate = DateTime.UtcNow,
                        MerchantId = merchantUserId,
                        CategoryId = categories[p.Category],
                        BrandId = brands[p.Brand],
                        OuterDiameterMM = p.OuterDiameterMM,
                        IsVisible = true,
                        WorkshopDeliveryEnabled = true
                    };
                    context.Products.Add(product);
                    await context.SaveChangesAsync();
                }

                foreach (var (make, model, year) in p.Compatibilities)
                {
                    var compatExists = await context.Compatibilities.AnyAsync(c =>
                        c.ProductId == product.Id && c.Make == make && c.Model == model && c.Year == year);

                    if (!compatExists)
                    {
                        context.Compatibilities.Add(new Compatibility
                        {
                            ProductId = product.Id,
                            Make = make,
                            Model = model,
                            Year = year
                        });
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
