using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace SqlDemo.Models
{
    // UserSqlRepository uses direct sql queries -- see UserContext for entity framework implementation
    public class IdentityUserSqlRepository : SqlRepository, IIdentityUserRepository
    {
        public IdentityUserSqlRepository(string connectionString) : base (connectionString) { }
        public DbSet<IdentityUser<Guid>> IdentityUser
        {
            get
            {
                return (DbSet<IdentityUser<Guid>>)GetUsers();
            } 
            set
            {
                throw new NotImplementedException();
            }
        }
        public DbSet<IdentityUserClaim<Guid>> IdentityUserClaim { 
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public DbSet<IdentityUserRole<Guid>> IdentityUserRole 
        { 
            get
            {
                throw new NotImplementedException();
            } 
            set
            {
                throw new NotImplementedException();
            } 
        }
       public  DbSet<IdentityRole<Guid>> IdentityRole 
       { 
           get
           {
                throw new NotImplementedException();
           }
           set
           {
                throw new NotImplementedException();
           } 
        }
        public void CreateUser(IdentityUser<Guid> user)
        {
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
            }
            // INSERT [dbo].[IdentityUser] ([Id], [Email], [UserName]) VALUES (user.Id, user.Email, user.UserName)
            Int32 result = ExecuteUnsafeNonQuery("INSERT [dbo].[IdentityUser] ([Id], [Email], [UserName]) VALUES (\'" +
                user.Id + "\', \'" + user.Email + "\', \'" + user.UserName + "\')");
            if (result != 1)
            {
                throw new InvalidOperationException("failed to create user");
            }
        }
        new private IEnumerable<IdentityUser<Guid>> ExecuteUnsafeQuery(string query)
        {
            //Console.WriteLine("\r\n UserRepository.ExecuteUnsafeQuery:: query={0} \r\n", query);
            using (SqlDataReader reader = base.ExecuteUnsafeQuery(query))
            {
                List<IdentityUser<Guid>> users = new List<IdentityUser<Guid>>();
                while (reader.Read())
                {
                    users.Add(new IdentityUser<Guid> 
                        {
                            Id = (Guid)reader["Id"],
                            Email = (string)reader["Email"], 
                            UserName = (string)reader["UserName"],
                            EmailConfirmed = (bool)reader["EmailConfirmed"],
                            PhoneNumber = (string)reader["PhoneNumber"],
                            PhoneNumberConfirmed = (bool)reader["PhoneNumberConfirmed"],
                            TwoFactorEnabled = (bool)reader["TwoFactorEnabled"]
                        });
                }
                return users;
            }
        }
        public IEnumerable<IdentityUser<Guid>> GetUsers()
        {
            return ExecuteUnsafeQuery("SELECT * FROM [dbo].[IdentityUser]");
        }
        public IdentityUser<Guid> GetUser(Guid userId)
        {
            // SELECT * FROM [dbo].[IdentityUser] U WHERE U.Id = userId
            return ExecuteUnsafeQuery("SELECT * FROM [dbo].[IdentityUser] U WHERE U.Id = \'" + userId + "\'").FirstOrDefault();
        }
        public IEnumerable<IdentityUser<Guid>> FindUsersByFirstAndOrLastName(string firstAndOrLastName)
        {
            //string firstName = firstAndOrLastName, lastName = firstAndOrLastName;
            //string op = "OR";
            //string[] names = firstAndOrLastName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            //if (names.Length == 2)
            //{
            //    firstName = names[0];
            //    lastName = names[1];
            //    op = "AND";
            //}
            // SELECT * FROM [dbo].[IdentityUser] U WHERE U.UserName = firstAndOrLastName
            return ExecuteUnsafeQuery("SELECT * FROM [dbo].[IdentityUser] U WHERE U.UserName = \'" + firstAndOrLastName + "\'");
        }
        public IEnumerable<IdentityUser<Guid>> FindUsersByEmail(string email)
        {
            // SELECT * FROM [dbo].[IdentityUser] U WHERE U.Email = email
            return ExecuteUnsafeQuery("SELECT * FROM [dbo].[IdentityUser] U WHERE U.Email = \'" + email + "\'");
        }
        public IEnumerable<IdentityUser<Guid>> FindUsersByClass(Guid classId, IEnrolementRepository enrolementRepository)
        {
            // SELECT U.Id, U.Email, U.UserName, EmailConfirmed, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled FROM [dbo].[IdentityUser] U JOIN [dbo].[Enrolement] E ON U.Id = E.UserId WHERE E.ClassId = classId
            return ExecuteUnsafeQuery("SELECT U.Id, U.Email, U.UserName, EmailConfirmed, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled FROM [dbo].[IdentityUser] U JOIN [dbo].[Enrolement] E ON U.Id = E.UserId WHERE E.ClassId = \'" + classId + "\'");
        }
        public void UpdateUser(IdentityUser<Guid> user)
        {
            // UPDATE [dbo].[IdentityUser] SET Email = user.Email, UserName = user.UserName WHERE Id = user.Id
            Int32 result = ExecuteUnsafeNonQuery("UPDATE [dbo].[IdentityUser] SET Email = \'" + user.Email +
                "\',UserName = \'" + user.UserName +
                 "\' WHERE Id = \'" + user.Id + "\'");
            if (result != 1)
            {
                throw new InvalidOperationException("failed to update user");
            }
        }
        public void DeleteUser(Guid userId)
        { 
            // DELETE FROM [dbo].[IdentityUser] WHERE Id = userId
            Int32 result = ExecuteUnsafeNonQuery("DELETE FROM [dbo].[IdentityUser] WHERE Id = \'" + userId + "\'");
            if (result != 1)
            {
                throw new InvalidOperationException("failed to delete user");
            }
        }
    }
}