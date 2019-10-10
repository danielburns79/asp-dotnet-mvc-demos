using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SqlDemo.Models;

namespace SqlDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly IClassRepository repository;

        public ClassesController(IClassRepository repository) => this.repository = repository;

        // POST: api/classes
        [HttpPost]
        public ActionResult<Class> PostClass(Class classEntity)
        {
            this.repository.CreateClass(classEntity);
            return CreatedAtAction("PostClass", new Class{ClassId = classEntity.ClassId}, classEntity);
        }

        // GET: api/classes
        [HttpGet]
        public ActionResult<IEnumerable<Class>> GetClasses()
        {
            return new ActionResult<IEnumerable<Class>>(this.repository.GetClasses());
        }

        // GET: api/classes/n
        [HttpGet("{id:guid}")]
        public ActionResult<Class> GetClass(Guid id)
        {
            var classEntity = this.repository.GetClass(id);
            if (classEntity == null)
            {
                return NotFound();
            }
            return classEntity;
        }

        [HttpGet("{className}")]
        public ActionResult<IEnumerable<Class>> GetClasses(string className)
        {
            return new ActionResult<IEnumerable<Class>>(this.repository.GetClasses(className));
        }

        // PUT: api/classes/n
        [HttpPut("{id:guid}")]
        public ActionResult PutClass(Guid id, Class classEntity)
        {
            if (id != classEntity.ClassId)
            {
                return BadRequest();
            }
            this.repository.UpdateClass(classEntity);
            return NoContent();
        }

        // DELETE: api/classes/n
        [HttpDelete("{id:guid}")]
        public ActionResult<Class> DeleteClass(Guid id)
        {
            var classEntity = this.repository.GetClass(id);
            if (classEntity == null)
            {
                return NotFound();
            }
            this.repository.DeleteClass(id);
            return classEntity;
        }
    }
}