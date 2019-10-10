using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SqlDemo.Models;

namespace SqlDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionRepository questionRepository;
        private readonly IEnrolementRepository enrolementRepository;
        private readonly IProblemRepository problemRepository;

        public QuestionsController(IQuestionRepository questionRepository, IEnrolementRepository enrolementRepository, IProblemRepository problemRepository)
        {
            this.questionRepository = questionRepository;
            this.enrolementRepository = enrolementRepository;
            this.problemRepository = problemRepository;
        }

        // POST: api/questions
        [HttpPost]
        public ActionResult<Question> PostQuestion(Question questionEntity)
        {
            this.questionRepository.CreateQuestion(questionEntity);
            return CreatedAtAction("PostQuestions", new Question{QuestionId = questionEntity.QuestionId}, questionEntity);
        }

        // GET: api/questions
        [HttpGet]
        public ActionResult<IEnumerable<Question>> GetQuestions()
        {
            return new ActionResult<IEnumerable<Question>>(this.questionRepository.GetQuestions());
        }

        // GET: api/questions/n
        [HttpGet("{id:guid}")]
        public ActionResult<Question> GetQuestion(Guid id)
        {
            var questionEntity = this.questionRepository.GetQuestion(id);
            if (questionEntity == null)
            {
                return NotFound();
            }
            return questionEntity;
        }

        [HttpGet("{question}")]
        public ActionResult<IEnumerable<Question>> GetQuestions(string question)
        {
            return new ActionResult<IEnumerable<Question>>(this.questionRepository.GetQuestions(question));
        }

        [HttpGet("class/{classId:guid}")]
        public ActionResult<IEnumerable<Question>> FindQuestionsByClass(Guid classId)
        {
            return new ActionResult<IEnumerable<Question>>(
                this.questionRepository.FindQuestionsByClass(
                    classId,
                    this.problemRepository
                )
            );
        }

        [HttpGet("student/{userId:guid}")]
        public ActionResult<IEnumerable<Question>> FindQuestionsByStudent(Guid userId)
        {
            return new ActionResult<IEnumerable<Question>>(
                this.questionRepository.FindQuestionsByStudent(
                    userId,
                    this.problemRepository
                )
            );
        }

        [HttpGet("teacher/{userId:guid}")]
        public ActionResult<IEnumerable<Question>> FindQuestionsByTeacher(Guid userId)
        {
            return new ActionResult<IEnumerable<Question>>(
                this.questionRepository.FindQuestionsByTeacher(
                    userId,
                    this.problemRepository,
                    this.enrolementRepository
                )
            );
        }

        // PUT: api/questions/n
        [HttpPut("{id:guid}")]
        public ActionResult PutQuestion(Guid id, Question questionEntity)
        {
            if (id != questionEntity.QuestionId)
            {
                return BadRequest();
            }
            this.questionRepository.UpdateQuestion(questionEntity);
            return NoContent();
        }

        // DELETE: api/questions/n
        [HttpDelete("{id:guid}")]
        public ActionResult<Question> DeleteQuestion(Guid id)
        {
            var questionEntity = this.questionRepository.GetQuestion(id);
            if (questionEntity == null)
            {
                return NotFound();
            }
            this.questionRepository.DeleteQuestion(id);
            return questionEntity;
        }
    }
}