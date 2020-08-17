using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Web;


namespace DALA
{
    
    public class DBManager
    {
        private SqlConnection _sqlConn;
        public DBManager(string conn)
        {
            
            var connectionString = ConfigurationManager.ConnectionStrings["ValuesTests"].ConnectionString;
            _sqlConn = new SqlConnection(connectionString);

        }
        
        public object AddNewProduct(ProductDTO product)
        {
            using (var cmd = new SqlCommand("AddProduct", _sqlConn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("@name", System.Data.SqlDbType.VarChar).Value = product.name;
                cmd.Parameters.Add("@price", System.Data.SqlDbType.Float).Value = product.price;
                cmd.Parameters.Add("@category", System.Data.SqlDbType.VarChar).Value = product.category;
                cmd.Parameters.Add("@description", System.Data.SqlDbType.VarChar).Value = product.description;
                cmd.Parameters.Add("@imageName", System.Data.SqlDbType.VarChar).Value = product.imageName;
                cmd.Parameters.Add("@default_path", System.Data.SqlDbType.VarChar).Value = ConfigurationManager.AppSettings["UserImagePath"].ToString();
                cmd.Parameters.Add("@product_id", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add("@image_id", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;


                _sqlConn.Open();
                cmd.ExecuteNonQuery();
                var product_id = Convert.ToInt32(cmd.Parameters["@product_id"].Value);
                var image_id = Convert.ToInt32(cmd.Parameters["@image_id"].Value);
                _sqlConn.Close();

                return new { productId=product_id, imageId=image_id };
            }
        }

 

        public ProductsWithPagination GetFilteredProducts(List<string> categories, string keywords, int? min, int? max, string orderBy, bool? sortAscending,int page)
        {
            var products = new List<ProductDTO>();
            int pages;
            using (var cmd=new SqlCommand("GetFilteredProducts",_sqlConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@totalPages", SqlDbType.Int).Direction = ParameterDirection.Output;
                var table = new DataTable();
                table.Columns.Add(new DataColumn("Category", typeof(string)));

                if (categories == null || categories.Count == 0)
                {
                    categories = GetCategories();
                }
                foreach (var category in categories)
                {
                    table.Rows.Add(category);
                }

                var parameter = cmd.Parameters.AddWithValue("@CategoriesList", table);
                parameter.SqlDbType = SqlDbType.Structured;
                parameter.TypeName = "CategoriesList";
                cmd.Parameters.Add("@keywords", SqlDbType.VarChar).Value = keywords ?? "";
                cmd.Parameters.Add("@min", SqlDbType.Int).Value = min;
                cmd.Parameters.Add("@max", SqlDbType.Int).Value = max;
                cmd.Parameters.Add("@orderBy", SqlDbType.VarChar).Value = orderBy;
                cmd.Parameters.Add("@sortAscending", SqlDbType.Bit).Value = sortAscending;
                cmd.Parameters.Add("@pg_no", SqlDbType.Int).Value = page;
                _sqlConn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            var product_id = reader.GetInt32(0);
                            var name = reader.GetString(1);
                            var price = reader.GetDouble(2);
                            var category = reader.GetString(3);
                            var description = reader.GetString(4);
                            var imageDir = reader.GetString(7);

                            products.Add(new ProductDTO() { name = name, price = price, description = description, category = category, product_id = product_id, imageName = imageDir });
                        }
                }
                pages = Convert.ToInt32(cmd.Parameters["@totalPages"].Value);
                _sqlConn.Close();
            }
            return new ProductsWithPagination { List = products, TotalPages = pages };
        }

        public List<ProductDTO> GetProductsByCategories(List<string> Categories)
        {
            var list = new List<ProductDTO>();
            using (var cmd=new SqlCommand("GetProductsByCategories",_sqlConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                var table = new DataTable();
                table.Columns.Add(new DataColumn("Category",typeof(string)));
                foreach (var category in Categories)
                {
                    table.Rows.Add(category);
                }

                var parameter = cmd.Parameters.AddWithValue("@CategoriesList", table);
                parameter.SqlDbType = SqlDbType.Structured;
                parameter.TypeName = "CategoriesList";

                _sqlConn.Open();
                using (var reader=cmd.ExecuteReader())
                {
                    if(reader.HasRows)
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
                _sqlConn.Close();
                
            }
            return list;

        }
        public List<string> GetCategories()
        {
            List<string> list=new List<string>();
            using (var cmd=new SqlCommand("GetCategories",_sqlConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                _sqlConn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var category = reader.GetString(0);
                        list.Add(category);
                    }
                }
                _sqlConn.Close();
            }
            return list;
        }
        public ProductsWithPagination GetProducts()
        {
            var products = new List<ProductDTO>();
            int pages;
            using (var cmd = new SqlCommand("GetProducts", _sqlConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@totalPages", SqlDbType.Int).Direction = ParameterDirection.Output;

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
                        var imageDir = reader.GetString(7);

                        products.Add(new ProductDTO() { name = name, price = price, description = description, category = category, product_id = product_id, imageName = imageDir });
                    }
                }
                reader.Close();
                 pages= Convert.ToInt32(cmd.Parameters["@totalPages"].Value);
                _sqlConn.Close();
            }
            return new ProductsWithPagination { List = products, TotalPages = pages };

        }

        public string DeleteProduct(int id)
        {
            string dir;
            using (var cmd = new SqlCommand("DeleteProduct", _sqlConn))
            {
                cmd.CommandType =CommandType.StoredProcedure;
                cmd.Parameters.Add("@product_id",SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@image_dir", SqlDbType.VarChar,255).Direction = ParameterDirection.Output;
                _sqlConn.Open();
                cmd.ExecuteNonQuery();
                dir = Convert.ToString(cmd.Parameters["@image_dir"].Value);
                _sqlConn.Close();
            }
            return dir;
        }  
        public List<ProductDTO> SearchByKeywords(string keywords)
        {
            
            var list = new List<ProductDTO>();
            using (var cmd = new SqlCommand("GetProductsByKeyword", _sqlConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@search", System.Data.SqlDbType.VarChar).Value = keywords;
                cmd.Parameters.Add("@totalResults", SqlDbType.Int).Direction = ParameterDirection.Output;

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
                        var image = reader.GetString(7);
                        list.Add(new ProductDTO() { product_id = product_id, name = name, category = category, price = price, description = description ,imageName = image});
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
                sqlCmd.CommandType = CommandType.StoredProcedure;
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

        public List<AddressDTO> GetUserAddresses(int UserId)
        {
            var list = new List<AddressDTO>();
            using (var cmd=new SqlCommand("GetUserAddresses",_sqlConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("UserId", SqlDbType.Int).Value = UserId;
                _sqlConn.Open();

                using (var reader=cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            list.Add(new AddressDTO
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                Address = reader.GetString(2),
                                Name = reader.GetString(3),
                                Email = reader.GetString(4),
                                Password = reader.GetString(5)
                            });
                        }
                    }
                }
                _sqlConn.Close();
            }
            return list;
        }

        public bool DeleteUserAddress(int UserId,int AddressId)
        {
            using (var cmd=new SqlCommand("DeleteUserAddress",_sqlConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                cmd.Parameters.Add("@AddressId", SqlDbType.Int).Value = AddressId;

                _sqlConn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                    _sqlConn.Close();
                    return true;
                }
                catch (Exception)
                {

                    _sqlConn.Close();
                    return false;
                }
                
            }
        }

        public int AddUserAddress(int UserId,string address)
        {
            int addressId;
            using (var cmd=new SqlCommand("AddUserAddress",_sqlConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                cmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = address;
                cmd.Parameters.Add("@AddressId", SqlDbType.Int).Direction = ParameterDirection.Output;

                
                _sqlConn.Open();
                cmd.ExecuteNonQuery();
                addressId=Convert.ToInt32(cmd.Parameters["@AddressId"].Value);
                _sqlConn.Close();
            }
            return addressId;
        }

        public bool CheckIfAdmin(string email)
        {
            bool isAdmin;
            using (var cmd= new SqlCommand("CheckIfAdmin",_sqlConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@isAdmin", SqlDbType.Bit).Direction = ParameterDirection.Output;
                _sqlConn.Open();
                cmd.ExecuteNonQuery();
                 isAdmin= Convert.ToBoolean(cmd.Parameters["@isAdmin"].Value);
                _sqlConn.Close();
            }
            return isAdmin;
        }
    }

}
