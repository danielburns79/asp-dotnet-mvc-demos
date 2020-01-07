using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlDemo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDemo.Models.Tests
{
    [TestClass()]
    public class AssignmentEntityFrameworkRepositoryTests
    {
        [TestMethod()]
        public void CreateAssignmentTest()
        {
            //AssignmentEntityFrameworkRepository repository = new AssignmentEntityFrameworkRepository(new DbContextOptionsBuilder<AssignmentEntityFrameworkRepository>().UseSqlServer("").Options);
            //Assignment assignment = new Assignment() { };
            //repository.CreateAssignment(assignment);
            //Assert.Fail();
        }
    }
}