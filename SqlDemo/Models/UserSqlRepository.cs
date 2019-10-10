using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SqlDemo.Models
{
    // UserSqlRepository uses direct sql queries -- see UserContext for entity framework implementation
    public class UserSqlRepository : SqlRepository, IUserRepository
    {
        public UserSqlRepository(string connectionString) : base (connectionString) { }
        public DbSet<User> Users
        {
            get
            {
                return (DbSet<User>)GetUsers(); 
            } 
            set
            {
                throw new NotImplementedException();
            }
        }
        public void CreateUser(User user)
        {
            if (user.UserId == Guid.Empty)
            {
                user.UserId = Guid.NewGuid();
            }
            // INSERT [dbo].[Users] ([UserId], [Email], [FirstName], [LastName]) VALUES (user.UserId, user.Email, user.FirstName, user.LastName)
            Int32 result = ExecuteUnsafeNonQuery("INSERT [dbo].[Users] ([UserId], [Email], [FirstName], [LastName]) VALUES (\'" +
                user.UserId + "\', \'" + user.Email + "\', \'" + user.FirstName + "\', \'" + user.LastName + "\')");
            if (result != 1)
            {
                throw new InvalidOperationException("failed to create user");
            }
        }
        new private IEnumerable<User> ExecuteUnsafeQuery(string query)
        {
            Console.WriteLine("\r\n UserRepository.ExecuteUnsafeQuery:: query={0} \r\n", query);
            using (SqlDataReader reader = base.ExecuteUnsafeQuery(query))
            {
                List<User> users = new List<User>();
                while (reader.Read())
                {
                    users.Add(new User 
                        {
                            UserId = (Guid)reader["UserId"],
                            Email = (string)reader["Email"], 
                            FirstName = (string)reader["FirstName"], 
                            LastName = (string)reader["LastName"] 
                        });
                }
                return users;
            }
        }
        public IEnumerable<User> GetUsers()
        {
            return ExecuteUnsafeQuery("SELECT * FROM [dbo].[Users]");
        }
        public User GetUser(Guid userId)
        {
            // SELECT * FROM [dbo].[Users] U WHERE U.UserId = userId
            return ExecuteUnsafeQuery("SELECT * FROM [dbo].[Users] U WHERE U.UserId = \'" + userId + "\'").FirstOrDefault();
        }
        public IEnumerable<User> FindUsersByFirstAndOrLastName(string firstAndOrLastName)
        {
            string firstName = firstAndOrLastName, lastName = firstAndOrLastName;
            string op = "OR";
            string[] names = firstAndOrLastName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (names.Length == 2)
            {
                firstName = names[0];
                lastName = names[1];
                op = "AND";
            }
            // SELECT * FROM [dbo].[Users] U WHERE U.Firstname = firstName [AND|OR] U.LastName = lastName
            return ExecuteUnsafeQuery("SELECT * FROM [dbo].[Users] U WHERE U.FirstName = \'" + firstName + "\' " + op + " U.LastName = \'" + lastName + "\'");
        }
        public IEnumerable<User> FindUsersByEmail(string email)
        {
            // SELECT * FROM [dbo].[Users] U WHERE U.Email = email
            return ExecuteUnsafeQuery("SELECT * FROM [dbo].[Users] U WHERE U.Email = \'" + email + "\'");
        }
        public IEnumerable<User> FindUsersByClass(Guid classId, IEnrolementRepository enrolementRepository)
        {
            // SELECT U.UserId, U.Email, U.FirstName, U.LastName FROM [dbo].[Users] U JOIN [dbo].[Enrolement] E ON U.UserId = E.UserId WHERE E.ClassId = classId
            return ExecuteUnsafeQuery("SELECT U.UserId, U.Email, U.FirstName, U.LastName FROM [dbo].[Users] U JOIN [dbo].[Enrolement] E ON U.UserId = E.UserId WHERE E.ClassId = \'" + classId + "\'");
        }
        public void UpdateUser(User user)
        {
            // UPDATE [dbo].[Users] SET Email = user.Email, FirstName = user.Firstname, LastName = user.LastName WHERE UserId = user.UserId
            Int32 result = ExecuteUnsafeNonQuery("UPDATE [dbo].[Users] SET Email = \'" + user.Email +
                "\', FirstName = \'" + user.FirstName + "\', LastName = \'" + user.LastName +
                 "\' WHERE UserId = \'" + user.UserId + "\'");
            if (result != 1)
            {
                throw new InvalidOperationException("failed to update user");
            }
        }
        public void DeleteUser(Guid userId)
        { 
            // DELETE FROM [dbo].[Users] WHERE UserId = userId
            Int32 result = ExecuteUnsafeNonQuery("DELETE FROM [dbo].[Users] WHERE UserId = \'" + userId + "\'");
            if (result != 1)
            {
                throw new InvalidOperationException("failed to delete user");
            }
        }
    }
}