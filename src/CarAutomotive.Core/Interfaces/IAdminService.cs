using CarAutomotive.Core.DTOs.AdminDtos;

namespace CarAutomotive.Core.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<UserDirectoryDto>> GetAllUsersDirectoryAsync(int pageNumber = 1, int pageSize = 10);
    }
}
