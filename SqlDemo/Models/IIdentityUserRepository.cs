using System;
using System.Collections.Generic;
using SqlDemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace SqlDemo.Models
{
    public interface IIdentityUserRepository
    {
        DbSet<IdentityUser<Guid>> IdentityUser { get; set; }
        DbSet<IdentityUserClaim<Guid>> IdentityUserClaim { get; set; }
        DbSet<IdentityUserRole<Guid>> IdentityUserRole { get; set; }
        DbSet<IdentityRole<Guid>> IdentityRole { get; set; }
        void CreateUser(IdentityUser<Guid> user);
        IEnumerable<IdentityUser<Guid>> GetUsers();
        IdentityUser<Guid> GetUser(Guid userId);
        IEnumerable<IdentityUser<Guid>> FindUsersByFirstAndOrLastName(string firstAndOrLastName);
        IEnumerable<IdentityUser<Guid>> FindUsersByEmail(string email);
        IEnumerable<IdentityUser<Guid>> FindUsersByClass(Guid classId, IEnrolementRepository enrolementRepository);
        void UpdateUser(IdentityUser<Guid> user);
        void DeleteUser(Guid userId);
    }
}