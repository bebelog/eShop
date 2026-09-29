using System.Collections.Generic;
using eShop.CoreBusiness.Models;
using eShop.UseCases.PluginInterfaces.DataStore;
using eShop.DataStore.SQL.Dapper.Helpers;

namespace eShop.DataStore.SQL.Dapper
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDataAccess dataAccess;

        public ProductRepository(IDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public Product? GetProduct(int id)
        {
            string sql = "SELECT * FROM Product WHERE ProductId = @ProductId";
            return dataAccess.QuerySingle<Product, dynamic>(sql, new { ProductId = id });
        }

        public IEnumerable<Product> GetProducts(string? filter = null)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                string sql = "SELECT * FROM Product";
                return dataAccess.Query<Product, dynamic>(sql, new { });
            }
            else
            {
                string sql = "SELECT * FROM Product WHERE Name LIKE '%' + @Filter + '%'";
                return dataAccess.Query<Product, dynamic>(sql, new { Filter = filter });
            }
        }
    }
}
