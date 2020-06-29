using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;


namespace DALA
{
    
    public class DBManager
    {
        private SqlConnection _sqlConn;
        public DBManager(string conn)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[conn].ConnectionString;
            _sqlConn = new SqlConnection(connectionString);

        }
        public int AddNewProduct(ProductDTO product)
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
                        var product_id = reader.GetInt32(0);
                        var name = reader.GetString(1);
                        var price = reader.GetDouble(2);
                        var category = reader.GetString(3);
                        var description = reader.GetString(4);
                        list.Add(new ProductDTO() { product_id = product_id, name = name, category = category, price = price, description = description });
                    }
                }
                _sqlConn.Close();
            }
            return list;
        }

        public ProductDTO GetProduct(int productId)
        {
            var product = new ProductDTO();
            using (var cmd = new SqlCommand("GetProductById", _sqlConn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@productId", System.Data.SqlDbType.Int).Value = productId;

                _sqlConn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        product.product_id = reader.GetInt32(0);
                        product.name = reader.GetString(1);
                        product.price = reader.GetDouble(2);
                        product.category = reader.GetString(3);
                        product.description = reader.GetString(4);

                    }
                }
                _sqlConn.Close();
            }
            return product;
        }

        public AuthenticationStatus AuthenticateUser(UserDto userDto)
        {
            AuthenticationStatus status;
            using (var cmd = new SqlCommand("AuthenticateUser", _sqlConn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@email", System.Data.SqlDbType.VarChar).Value = userDto.Email;
                cmd.Parameters.Add("@password", System.Data.SqlDbType.VarChar).Value = userDto.Password;
                cmd.Parameters.Add("@userId", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add("@status", System.Data.SqlDbType.TinyInt).Direction = System.Data.ParameterDirection.Output;

                _sqlConn.Open();
                cmd.ExecuteNonQuery();
                status = (AuthenticationStatus)Convert.ToInt16(cmd.Parameters["@status"].Value);
                _sqlConn.Close();

            }
            return status;
        }

        public UserDto GetUser(string email)
        {
            UserDto userDto = new UserDto();
            using (var cmd = new SqlCommand("GetUser", _sqlConn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@email", System.Data.SqlDbType.VarChar).Value = email;

                _sqlConn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        userDto.UserId = reader.GetInt32(0);
                        userDto.Name = reader.GetString(1);
                        userDto.Email = reader.GetString(2);
                        userDto.Password = reader.GetString(3);

                    }
                }
                _sqlConn.Close();

            }
            return userDto;
        }
        public List<ProductDTO> GetProductsByIds(List<int> productIds)
        {
            var list = new List<ProductDTO>();
            using (var sqlCmd = new SqlCommand("GetProductsByIds", _sqlConn))
            {
                var IdsTable = new DataTable();
                IdsTable.Columns.Add(new DataColumn("ID", typeof(int)));
                foreach (var id in productIds)
                {
                    IdsTable.Rows.Add(id);
                }
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                var parameter = sqlCmd.Parameters.AddWithValue("@IDslist", IdsTable);
                parameter.SqlDbType = SqlDbType.Structured;
                parameter.TypeName = "dbo.IDList";
                _sqlConn.Open();
                using (var reader = sqlCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var product_id = reader.GetInt32(0);
                            var name = reader.GetString(1);
                            var price = reader.GetDouble(2);
                            var category = reader.GetString(3);
                            var description = reader.GetString(4);

                            list.Add(new ProductDTO()
                            {
                                product_id = product_id,
                                name = name,
                                price = price,
                                category = category,
                                description = description
                            });
                        }
                    }
                }
                _sqlConn.Close();
            }
            return list;
        }

        public int PurchaseProducts(int UserId, List<ProductPurchaseDTO> list,int Total)
        {
            int orderId;
            try
            {
                using (var cmd = new SqlCommand("Purchase", _sqlConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var ProductsTable = new DataTable();
                    ProductsTable.Columns.Add(new DataColumn("ID", typeof(int)));
                    ProductsTable.Columns.Add(new DataColumn("quantity", typeof(int)));

                    foreach (var item in list)
                    {
                        var row = ProductsTable.NewRow();
                        row["ID"] = item.product_id;
                        row["quantity"] = item.quantity;
                        ProductsTable.Rows.Add(row);
                    }


                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                    cmd.Parameters.Add("@Amount", SqlDbType.Int).Value = Total;
                    cmd.Parameters.Add("@IdOrder", SqlDbType.Int).Direction = ParameterDirection.Output;
                    

                    var param = cmd.Parameters.AddWithValue("@list", ProductsTable);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = "Products";

                    _sqlConn.Open();
                    cmd.ExecuteNonQuery();
                    orderId = Convert.ToInt32(cmd.Parameters["@IdOrder"].Value);
                    _sqlConn.Close();
                }
                return orderId;
            }
            catch (Exception)
            {
                return -1;
                throw;
            }


        }
        public List<OrderDetailDTO> GetOrdersList(int UserId)
        {
            List<OrderDetailDTO> ordersList = new List<OrderDetailDTO>();
            using (var cmd=new SqlCommand("GetOrdersList", _sqlConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;

                _sqlConn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int prevOrderId=-1;
                        OrderDetailDTO orderDetail=new OrderDetailDTO();
                        while (reader.Read())
                        {
                            var orderId = reader.GetInt32(0);
                            if (prevOrderId == -1)
                            {
                                prevOrderId = orderId;
                                orderDetail = new OrderDetailDTO();
                                orderDetail.Amount = reader.GetInt32(1);
                                orderDetail.Date = reader.GetDateTime(2);
                                orderDetail.Order_Id = orderId;
                                ordersList.Add(orderDetail);
                            }

                            if (prevOrderId == orderId)
                            {
                                var product = new OrderProductDTO();

                                product.quantity = reader.GetInt32(3);
                                product.price = reader.GetDouble(4);
                                product.category = reader.GetString(5);
                                product.description = reader.GetString(6);
                                product.name = reader.GetString(7);
                                
                                orderDetail.Products.Add(product);
                            }
                            else
                            {
                                orderDetail = new OrderDetailDTO();
                                orderDetail.Amount = reader.GetInt32(1);
                                orderDetail.Date = reader.GetDateTime(2);
                                orderDetail.Order_Id = orderId;

                                orderDetail.Products.Add(new OrderProductDTO()
                                {
                                    price = reader.GetDouble(4),
                                    category = reader.GetString(5),
                                    description = reader.GetString(6),
                                    quantity = reader.GetInt32(3),
                                    name = reader.GetString(7)
                                });
                                ordersList.Add(orderDetail);

                            }
                            prevOrderId = orderId;

                        }
                    }
                }
                _sqlConn.Close();
            }
            return ordersList;
        }

        public OrderDetailDTO GetOrderById(int orderId)
        {
            using(var cmd=new SqlCommand("GetOrderById",_sqlConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;

                var orderDetail = new OrderDetailDTO();
                _sqlConn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            orderDetail.Order_Id = reader.GetInt32(0);
                            orderDetail.Amount = reader.GetInt32(1);
                            orderDetail.Date = reader.GetDateTime(2);

                            orderDetail.Products.Add(new OrderProductDTO()
                            {
                                price = reader.GetDouble(4),
                                category = reader.GetString(5),
                                description = reader.GetString(6),
                                quantity = reader.GetInt32(3),
                                name = reader.GetString(7)
                            }
                            );
                        }
                    }
                }
                _sqlConn.Close();
                return orderDetail;

            }
        }


    }

}
