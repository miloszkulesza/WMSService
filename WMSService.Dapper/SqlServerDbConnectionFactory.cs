using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WMSService.Dapper
{
    /// <summary>
    /// Creates a new <see cref="SqlConnection"/> instance for connecting to Microsoft SQL Server.
    /// </summary>
    public class SqlServerDbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// The connection string to use for connecting to Microsoft SQL Server.
        /// </summary>
        public string ConnectionString { get; set; }

        public SqlServerDbConnectionFactory(IConfiguration _configuration)
        {
            configuration = _configuration;
            ConnectionString = configuration.GetConnectionString("WMSService");
        }

        /// <inheritdoc/>
        public IDbConnection Create()
        {
            var sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            return sqlConnection;
        }
    }
}
