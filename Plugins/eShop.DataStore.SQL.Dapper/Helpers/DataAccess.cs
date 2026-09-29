using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace eShop.DataStore.SQL.Dapper.Helpers
{
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("Default") 
                ?? "Server=localhost;Database=eShop;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public void ExecuteCommand<U>(string sql, U parameters)
        {
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                conn.Execute(sql, parameters);
            }
        }

        public List<T> Query<T, U>(string sql, U parameters)
        {
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                return conn.Query<T>(sql, parameters).ToList();
            }
        }

        public T? QueryFirst<T, U>(string sql, U parameters)
        {
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                return conn.QueryFirstOrDefault<T>(sql, parameters);
            }
        }

        public T? QuerySingle<T, U>(string sql, U parameters)
        {
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                return conn.QuerySingleOrDefault<T>(sql, parameters);
            }
        }
    }
}
