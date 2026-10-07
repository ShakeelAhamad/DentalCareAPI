using DentalCareAPI.Models;

namespace DentalCareAPI.Repositories
{
    public interface IBlockedDateRepository : IRepository<BlockedDate>
    {
        //----------------------------------------------------------
        //BLOCKEDDATE-SPECIFIC QUERIES
        //These are additional to what IRepository<BlockedDate> provides
        //----------------------------------------------------------
        Task<bool> ExistsAsync(int dentistId, DateOnly date);
    }
}
