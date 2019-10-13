using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SqlDemo.Models;

namespace SqlDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentRepository assignmentRepository;
        private readonly IEnrolementRepository enrolementRepository;
        private readonly IProblemRepository problemRepository;

        public AssignmentsController(IAssignmentRepository assignmentRepository, IEnrolementRepository enrolementRepository, IProblemRepository problemRepository)
        {
            this.assignmentRepository = assignmentRepository;
            this.enrolementRepository = enrolementRepository;
            this.problemRepository = problemRepository;
        }

        // POST: api/assignments
        [HttpPost]
        public ActionResult<Assignment> PostAssignment(Assignment assignmentEntity)
        {
            this.assignmentRepository.CreateAssignment(assignmentEntity);
            return CreatedAtAction("PostAssignment", new Assignment{AssignmentId = assignmentEntity.AssignmentId}, assignmentEntity);
        }

        // GET: api/assignments
        [HttpGet]
        public ActionResult<IEnumerable<Assignment>> GetAssignments()
        {
            return new ActionResult<IEnumerable<Assignment>>(this.assignmentRepository.GetAssignments());
        }

        // GET: api/assignments/n
        [HttpGet("{id:guid}")]
        public ActionResult<Assignment> GetAssignment(Guid id)
        {
            var assignmentEntity = this.assignmentRepository.GetAssignment(id);
            if (assignmentEntity == null)
            {
                return NotFound();
            }
            return assignmentEntity;
        }

        [HttpGet("class/{classId:guid}")]
        public ActionResult<IEnumerable<Assignment>> FindAssignmentsByClass(Guid classId)
        {
            return new ActionResult<IEnumerable<Assignment>>(
                this.assignmentRepository.FindAssignmentsByClass(
                    classId,
                    this.problemRepository
                )
            );
        }

        [HttpGet("student/{userId:guid}")]
        public ActionResult<IEnumerable<Assignment>> FindAssignmentsByUser(Guid userId)
        {
            return new ActionResult<IEnumerable<Assignment>>(
                this.assignmentRepository.FindAssignmentsByStudent(
                    userId, 
                    this.problemRepository
                )
            );
        }

        [HttpGet("teacher/{userId:guid}")]
        public ActionResult<IEnumerable<Assignment>> FindAssignmentsByTeacher(Guid userId)
        {
            return new ActionResult<IEnumerable<Assignment>>(
                this.assignmentRepository.FindAssignmentsByTeacher(
                    userId, 
                    this.problemRepository,
                    this.enrolementRepository
                )
            );
        }

        // PUT: api/assignments/n
        [HttpPut("{id:guid}")]
        public ActionResult PutAssignment(Guid id, Assignment assignmentEntity)
        {
            if (id != assignmentEntity.AssignmentId)
            {
                return BadRequest();
            }
            this.assignmentRepository.UpdateAssignment(assignmentEntity);
            return NoContent();
        }

        // DELETE: api/assignments/n
        [HttpDelete("{id:guid}")]
        public ActionResult<Assignment> DeleteAssignment(Guid id)
        {
            var assignmentEntity = this.assignmentRepository.GetAssignment(id);
            if (assignmentEntity == null)
            {
                return NotFound();
            }
            this.assignmentRepository.DeleteAssignment(id);
            return assignmentEntity;
        }
    }
}