using System;
using System.Collections.Generic;
using SqlDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace SqlDemo.Models
{
    public interface IUserRepository
    {
        DbSet<User> Users { get; set; }
        void CreateUser(User user);
        IEnumerable<User> GetUsers();
        User GetUser(Guid UserId);
        IEnumerable<User> FindUsersByFirstAndOrLastName(string firstAndOrLastName);
        IEnumerable<User> FindUsersByEmail(string email);
        IEnumerable<User> FindUsersByClass(Guid classId, IEnrolementRepository enrolementRepository);
        void UpdateUser(User user);
        void DeleteUser(Guid userId);
    }
}