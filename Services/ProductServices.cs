using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBase_MVVM.Models;

namespace DataBase_MVVM.Services
{
    public class ProductService
    {
        private readonly string _connectionString;

        public ProductService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT ProductID, ProductName, UnitPrice, SupplierID, CategoryID, " +
                             "QuantityPerUnit, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued " +
                             "FROM Products";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ProductId = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            UnitPrice = reader.IsDBNull(2) ? (decimal?)null : reader.GetDecimal(2),
                            SupplierId = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                            CategoryId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                            QuantityPerUnit = reader.IsDBNull(5) ? null : reader.GetString(5),
                            UnitsInStock = reader.IsDBNull(6) ? (short?)null : reader.GetInt16(6),
                            UnitsOnOrder = reader.IsDBNull(7) ? (short?)null : reader.GetInt16(7),
                            ReorderLevel = reader.IsDBNull(8) ? (short?)null : reader.GetInt16(8),
                            Discontinued = reader.GetBoolean(9)
                        });
                    }
                }
            }

            return products;
        }

        public bool UpdateProduct(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE Products SET ProductName = @Name, UnitPrice = @Price " +
                             "WHERE ProductID = @ID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", product.ProductId);
                    command.Parameters.AddWithValue("@Name", product.ProductName);
                    command.Parameters.AddWithValue("@Price", (object)product.UnitPrice ?? DBNull.Value);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteProduct(int productId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Products WHERE ProductID = @ID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", productId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
