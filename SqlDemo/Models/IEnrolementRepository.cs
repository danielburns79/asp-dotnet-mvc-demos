using System;
using System.Collections.Generic;
using SqlDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace SqlDemo.Models
{
    public interface IEnrolementRepository
    {
        DbSet<Enrolement> Enrolement { get; set; }
        void CreateEnrolement(Enrolement enrolement);
        IEnumerable<Enrolement> GetEnrolement();
        IEnumerable<Enrolement> FindEnrolementByClassId(Guid classId);
        IEnumerable<Enrolement> FindEnrolementByUserId(Guid UserId);
        void UpdateEnrolement(Enrolement enrolement);
        void DeleteEnrolement(Enrolement enrolement);
    }
}