using DentalCareAPI.Data;
using DentalCareAPI.Models;

namespace DentalCareAPI.Repositories
{
    public class BlockedDateRepository : Repository<BlockedDate>,IBlockedDateRepository
    {
        public BlockedDateRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        public async Task<bool> ExistsAsync(int dentistId, DateOnly date)
        {
            return await AnyAsync(bd => bd.DentistId == dentistId && bd.BlockDate == date);
        }
    }
}
