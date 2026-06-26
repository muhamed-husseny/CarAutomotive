using CarAutomotive.Core.DTOs.AdminDtos;
using CarAutomotive.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarAutomotive.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IGenericRepository<MechanicProfile> _mechanicRepo;
        private readonly IGenericRepository<Merchants> _merchantRepo;
        private readonly UserManager<AppUser> _userManager;

        public AdminService(
            IGenericRepository<MechanicProfile> mechanicRepo,
            IGenericRepository<Merchants> merchantRepo,
            UserManager<AppUser> userManager)
        {
            _mechanicRepo = mechanicRepo;
            _merchantRepo = merchantRepo;
            _userManager = userManager;
        }

        public async Task<IEnumerable<UserDirectoryDto>> GetAllUsersDirectoryAsync(int pageNumber = 1, int pageSize = 10)
        {
            var users = await _userManager.Users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var allMechanics = await _mechanicRepo.GetAllAsync();
            var mechanics = allMechanics.ToDictionary(m => m.UserId);

            var allMerchants = await _merchantRepo.GetAllAsync();
            var merchants = allMerchants.ToDictionary(m => m.AppUserId);

            var directory = new List<UserDirectoryDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault() ?? "Client";

                string businessName = null;
                string status = user.IsActive ? "ACTIVE" : "SUSPENDED";

                if (userRole == "Mechanic" && mechanics.TryGetValue(user.Id, out var mechanicData))
                {
                    businessName = mechanicData.Name;

                    if (user.IsActive)
                    {
                        status = mechanicData.IsAvailable ? "ACTIVE" : "PENDING VETTING";
                    }
                }
                else if (userRole == "Merchant" && merchants.TryGetValue(user.Id, out var merchantData))
                {
                    businessName = merchantData.ShopName;

                    if (user.IsActive)
                    {
                        status = merchantData.Status ?? "PENDING VETTING";
                    }
                }

                directory.Add(new UserDirectoryDto
                {
                    AccountId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = userRole,
                    Status = status,
                    BusinessName = businessName
                });
            }

            return directory;
        }
    }
}