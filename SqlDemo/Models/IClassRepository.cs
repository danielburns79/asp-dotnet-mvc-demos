using System;
using System.Collections.Generic;
using SqlDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace SqlDemo.Models
{
    public interface IClassRepository
    {
        DbSet<Class> Classes { get; set; }
        void CreateClass(Class classEntity);
        IEnumerable<Class> GetClasses();
        Class GetClass(Guid ClassId);
        IEnumerable<Class> GetClasses(string className);
        void UpdateClass(Class classEntity);
        void DeleteClass(Guid ClassId);
    }
}