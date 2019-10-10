using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SqlDemo.Models;

namespace SqlDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolementController : ControllerBase
    {
        private readonly IEnrolementRepository repository;

        public EnrolementController(IEnrolementRepository repository) => this.repository = repository;

        // POST: api/enrolement
        [HttpPost]
        public ActionResult<Enrolement> PostEnrolement(Enrolement enrolement)
        {
            this.repository.CreateEnrolement(enrolement);
            return CreatedAtAction("PostEnrolement", new Enrolement{ClassId = enrolement.ClassId, UserId = enrolement.UserId}, enrolement);
        }

        // GET: api/enrolement
        [HttpGet]
        public ActionResult<IEnumerable<Enrolement>> GetEnrolement()
        {
            return new ActionResult<IEnumerable<Enrolement>>(this.repository.GetEnrolement());
        }

        // GET: api/enrolement/class/n
        [HttpGet("class/{classId:guid}")]
        public ActionResult<IEnumerable<Enrolement>> GetEnrolementByClassId(Guid classId)
        {
            return new ActionResult<IEnumerable<Enrolement>>(this.repository.FindEnrolementByClassId(classId));
        }

        // GET: api/enrolement/user/n
        [HttpGet("user/{userId:guid}")]
        public ActionResult<IEnumerable<Enrolement>> GetEnrolementByUserId(Guid userId)
        {
            return new ActionResult<IEnumerable<Enrolement>>(this.repository.FindEnrolementByUserId(userId));
        }

        // PUT: api/enrolement
        [HttpPut()]
        public ActionResult PutEnrolement(Enrolement enrolement)
        {
            this.repository.UpdateEnrolement(enrolement);
            return NoContent();
        }

        // DELETE: api/enrolement
        [HttpDelete()]
        public ActionResult<Enrolement> DeleteEnrolement(Enrolement enrolement)
        {
            this.repository.DeleteEnrolement(enrolement);
            return enrolement;
        }
    }
}