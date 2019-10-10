using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlDemo.Models
{
    // UserEntityFrameworkRepository uses entity framework -- see UserSqlRepository for direct sql query example
    public class UserEntityFrameworkRepository : DbContext, IUserRepository
    {
        public UserEntityFrameworkRepository(DbContextOptions<UserEntityFrameworkRepository> options)
         : base (options)
        { }

        public DbSet<User> Users { get; set; }

        public void CreateUser(User user)
        {
            this.Users.Add(user);
            this.SaveChanges();
        }
        public IEnumerable<User> GetUsers()
        {
            return this.Users;
        }
        public User GetUser(Guid UserId)
        {
            return this.Users.Find(UserId);
        }
        public IEnumerable<User> FindUsersByFirstAndOrLastName(string firstAndOrLastName)
        {
            string firstName = firstAndOrLastName, lastName = firstAndOrLastName;
            string[] names = firstAndOrLastName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (names.Length == 2)
            {
                firstName = names[0];
                lastName = names[1];
                return this.Users.Where(u => u.FirstName == firstName && u.LastName == lastName);
            }
            return this.Users.Where(u => u.FirstName == firstAndOrLastName || u.LastName == firstAndOrLastName);
        }
        public IEnumerable<User> FindUsersByEmail(string email)
        {
            return this.Users.Where(u => u.Email == email);
        }
        public IEnumerable<User> FindUsersByClass(Guid classId, IEnrolementRepository enrolementRepository)
        {
            // must pull one of the tables into local memory with .ToList() before executing the join
            // otherwise, entity framework will throw an argumentnullexception
            // calling .ToList() on users executes the query
            /*return (
                from u in this.Users.ToList()
                join e in enrolementRepository.Enrolement
                on u.UserId equals e.UserId
                where e.ClassId == classId
                select new User {
                    UserId = u.UserId, 
                    Email = u.Email, 
                    FirstName = u.FirstName, 
                    LastName = u.LastName
                    }
                ).ToList();*/
            // the following does the same as the above but using the lambda syntax instead of the linq syntax
            /* return this.Users.ToList().Join(
                enrolementRepository.Enrolement, 
                u => u.UserId, 
                e => e.UserId, 
                (u, e) => new User{UserId = u.UserId, Email = u.Email, FirstName = u.FirstName, LastName = u.LastName}
                ).ToList();*/
            // the following executes the join on the sql server instead of pulling the table into local memory and then doing a linq join
            UserSqlRepository userSqlRepository = new UserSqlRepository(this.Database.GetDbConnection().ConnectionString);
            return userSqlRepository.FindUsersByClass(classId, enrolementRepository);
        }
        public void UpdateUser(User user)
        {
            this.Entry(user).State = EntityState.Modified;
            this.SaveChanges();
        }
        public void DeleteUser(Guid userId)
        {
            var user = this.Users.Find(userId);
            if (user == null)
            {
                throw new InvalidOperationException("failed to delete user");
            }
            this.Users.Remove(user);
            this.SaveChanges();
        }
    }
}