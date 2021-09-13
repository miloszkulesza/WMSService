using AspNetCore.Identity.Dapper;
using Dapper;
using System.Data;
using System.Threading.Tasks;
using WMSService.Dapper.Abstract;
using WMSService.Dapper.Models;

namespace WMSService.Dapper.Tables
{
    public class UserProfilesTable : TableBase, IUserProfilesTable
    {
        public IDbConnection DbConnection { get; set; }

        public UserProfilesTable(IDbConnectionFactory _dbConnectionFactory)
        {
            DbConnection = _dbConnectionFactory.Create();
        }

        protected override void OnDispose()
        {
            if (DbConnection != null)
                DbConnection.Dispose();
        }

        public async Task<bool> CreateAsync(UserProfile userProfile)
        {
            const string sql = "INSERT INTO [dbo].[UserProfiles] " +
                              "VALUES (@FirstName, @LastName, @UserId);";
            var rowsInserted = await DbConnection.ExecuteAsync(sql, new
            {
                userProfile.FirstName,
                userProfile.LastName,
                userProfile.UserId
            });
            return rowsInserted == 1;
        }
    }
}
