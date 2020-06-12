using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;


namespace DAL.Models
{
    public class DBManager
    {
        private SqlConnection _sqlConn;
        public DBManager(string conn)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[conn].ConnectionString;
            _sqlConn = new SqlConnection(connectionString);

        }
        public int AddToCart(ProductDTO product)
        {
            using (var cmd = new SqlCommand("AddProduct", _sqlConn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("@name", System.Data.SqlDbType.VarChar).Value = product.name;
                cmd.Parameters.Add("@price", System.Data.SqlDbType.Float).Value = product.price;
                cmd.Parameters.Add("@category", System.Data.SqlDbType.VarChar).Value = product.category;
                cmd.Parameters.Add("@description", System.Data.SqlDbType.VarChar).Value = product.description;
                cmd.Parameters.Add("@product_id", System.Data.SqlDbType.TinyInt).Direction = System.Data.ParameterDirection.Output;


                _sqlConn.Open();
                cmd.ExecuteNonQuery();
                var product_id = Convert.ToInt32(cmd.Parameters["@product_id"].Value);
                _sqlConn.Close();

                return product_id;
            }
        }
        public List<ProductDTO> GetProducts()
        {
            var products = new List<ProductDTO>();
            using (var cmd = new SqlCommand("GetProducts", _sqlConn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                _sqlConn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var product_id = reader.GetInt32(0);
                        var name = reader.GetString(1);
                        var price = reader.GetDouble(2);
                        var category = reader.GetString(3);
                        var description = reader.GetString(4);

                        products.Add(new ProductDTO() { name = name, price = price, description = description, category = category, product_id = product_id });
                    }
                }
                _sqlConn.Close();
            }
            return products;

        }

        public void DeleteProduct(int id)
        {
            using (var cmd = new SqlCommand("DeleteProduct", _sqlConn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@product_id", System.Data.SqlDbType.Int).Value = id;
                _sqlConn.Open();
                cmd.ExecuteNonQuery();
                _sqlConn.Close();
            }
        }
        public List<ProductDTO> SearchByKeywords(string keywords)
        {
            var list = new List<ProductDTO>();
            using (var cmd = new SqlCommand("GetProductsByKeyword", _sqlConn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@search", System.Data.SqlDbType.VarChar).Value = keywords;

                _sqlConn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString(1);
                        var price = reader.GetDouble(2);
                        var category = reader.GetString(3);
                        var description = reader.GetString(4);
                        list.Add(new ProductDTO() { name = name, category = category, price = price, description = description });
                    }
                }
                _sqlConn.Close();
            }
            return list;
        }
    }
}