using System.Data;
using System.Threading.Tasks;
using WMSService.Dapper.Models;

namespace WMSService.Dapper.Abstract
{
    public interface IUserProfilesTable
    {
        IDbConnection DbConnection { get; set; }
        Task<bool> CreateAsync(UserProfile user);
    }
}
