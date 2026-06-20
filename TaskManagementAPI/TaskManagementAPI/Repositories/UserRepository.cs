using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

       public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "SELECT UserId,UserName,Email FROM Users";
                using (var cmd =new SqlCommand(sql, connection)) 
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserId = reader.GetInt32(0),
                            UserName = reader.GetString(1),
                            Email = reader.GetString(2)
                        });
                    }
                }
            }
        }

        public User? GetUserById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "SELECT UserId, UserName, Email FROM Users WHERE UserId = @UserId";
                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = reader.GetInt32(0),
                                UserName = reader.GetString(1),
                                Email = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public int AddUser(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"
                    INSERT INTO Users (UserName, Email) 
                    VALUES (@UserName, @Email);
                    SELECT SCOPE_IDENTITY();";

                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
    }
}
