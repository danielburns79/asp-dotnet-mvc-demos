using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlDemo.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace SqlDemo.Controllers.Tests
{
    [TestClass()]
    public class AssignmentsControllerTests
    {
        [TestMethod()]
        public void GetAssignmentsTest()
        {
            Mock<Models.IAssignmentRepository> assignmentRepository = new Mock<Models.IAssignmentRepository>();
            Mock<Models.IEnrolementRepository> enrolementRepository = new Mock<Models.IEnrolementRepository>();
            Mock<Models.IProblemRepository> problemRepository = new Mock<Models.IProblemRepository>();
            AssignmentsController controller = new AssignmentsController(assignmentRepository.Object, enrolementRepository.Object, problemRepository.Object);
            assignmentRepository.Setup(repository => repository.GetAssignments()).Returns(new List<Models.Assignment>() { });
            ActionResult<IEnumerable<Models.Assignment>> result = controller.GetAssignments();
            Assert.AreEqual(0, result.Value.ToArray().Length, "result.Length");
        }
    }
}