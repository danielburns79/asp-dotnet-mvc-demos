using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SqlDemo.Models;

namespace SqlDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProblemsController : ControllerBase
    {
        private readonly IProblemRepository problemRepository;
        private readonly IEnrolementRepository enrolementRepository;
        private readonly IAssignmentRepository assignmentRepository;

        public ProblemsController(
            IProblemRepository problemRepository,
            IEnrolementRepository enrolementRepository,
            IAssignmentRepository assignmentRepository)
        {
            this.problemRepository = problemRepository;
            this.enrolementRepository = enrolementRepository;
            this.assignmentRepository = assignmentRepository;
        }

        // POST: api/problems
        [HttpPost]
        public ActionResult<Problem> PostProblem(Problem problemEntity)
        {
            this.problemRepository.CreateProblem(problemEntity);
            return CreatedAtAction("PostProblems", new Problem{ProblemId = problemEntity.ProblemId}, problemEntity);
        }

        // GET: api/problems
        [HttpGet]
        public ActionResult<IEnumerable<Problem>> GetProblems()
        {
            return new ActionResult<IEnumerable<Problem>>(this.problemRepository.GetProblems());
        }

        // GET: api/problems/n
        [HttpGet("{id:guid}")]
        public ActionResult<Problem> GetProblem(Guid id)
        {
            var problemEntity = this.problemRepository.GetProblem(id);
            if (problemEntity == null)
            {
                return NotFound();
            }
            return problemEntity;
        }

        [HttpGet("question/{questionId:guid}")]
        public ActionResult<IEnumerable<Problem>> FindProblemsByQuestion(Guid questionId)
        {
            return new ActionResult<IEnumerable<Problem>>(this.problemRepository.FindProblemsByQuestion(questionId));
        }

        [HttpGet("class/{classId:guid}")]
        public ActionResult<IEnumerable<Problem>> FindProblemsByClass(Guid classId)
        {
            return new ActionResult<IEnumerable<Problem>>(this.problemRepository.FindProblemsByClass(classId));
        }

        [HttpGet("student/{userId:guid}")]
        public ActionResult<IEnumerable<Problem>> FindProblemsByUser(Guid userId)
        {
            return new ActionResult<IEnumerable<Problem>>(this.problemRepository.FindProblemsByStudent(userId));
        }

        [HttpGet("teacher/{userId:guid}")]
        public ActionResult<IEnumerable<Problem>> FindProblemsByTeacher(Guid userId)
        {
            return new ActionResult<IEnumerable<Problem>>(this.problemRepository.FindProblemsByTeacher(userId, this.enrolementRepository));
        }

        [HttpGet("assignment/{assignmentId:guid}")]
        public ActionResult<IEnumerable<Problem>> FindProblemsByAssignment(Guid assignmentId)
        {
            return new ActionResult<IEnumerable<Problem>>(this.problemRepository.FindProblemsByAssignment(assignmentId, this.assignmentRepository));
        }

        // PUT: api/problems/n
        [HttpPut("{id:guid}")]
        public ActionResult PutProblem(Guid id, Problem problemEntity)
        {
            if (id != problemEntity.ProblemId)
            {
                return BadRequest();
            }
            this.problemRepository.UpdateProblem(problemEntity);
            return NoContent();
        }

        // DELETE: api/problems/n
        [HttpDelete("{id:guid}")]
        public ActionResult<Problem> DeleteProblem(Guid id)
        {
            var problemEntity = this.problemRepository.GetProblem(id);
            if (problemEntity == null)
            {
                return NotFound();
            }
            this.problemRepository.DeleteProblem(id);
            return problemEntity;
        }
    }
}