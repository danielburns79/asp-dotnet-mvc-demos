using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SqlDemo.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace SqlDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IIdentityUserRepository identityUserRepository;
        private readonly IClassRepository classRepository;
        private readonly IEnrolementRepository enrolementRepository;

        public UsersController(IIdentityUserRepository identityUserRepository, IClassRepository classRepository, IEnrolementRepository enrolementRepository)
        {
            this.identityUserRepository = identityUserRepository;
            this.classRepository = classRepository;
            this.enrolementRepository = enrolementRepository;
        }

        // POST: api/users
        [HttpPost]
        public ActionResult<IdentityUser<Guid>> PostUser(IdentityUser<Guid> user)
        {
            this.identityUserRepository.CreateUser(user);
            return CreatedAtAction("PostUser", new IdentityUser<Guid>{Id = user.Id}, user);
        }

        // GET: api/users
        //[Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult<IEnumerable<IdentityUser<Guid>>> GetUsers()
        {
            return new ActionResult<IEnumerable<IdentityUser<Guid>>>(this.identityUserRepository.GetUsers());
        }

        // GET: api/users/n
        [HttpGet("{id:guid}")]
        public ActionResult<IdentityUser<Guid>> GetUser(Guid id)
        {
            var user = this.identityUserRepository.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        // GET: api/users/nameoremail
        [HttpGet("{nameOrEmail}")]
        public ActionResult<IEnumerable<IdentityUser<Guid>>> GetUsers(string nameOrEmail)
        {
            if (nameOrEmail.Contains("@"))
            {
                return new ActionResult<IEnumerable<IdentityUser<Guid>>>(this.identityUserRepository.FindUsersByEmail(nameOrEmail));
            }
            else
            {
                return new ActionResult<IEnumerable<IdentityUser<Guid>>>(this.identityUserRepository.FindUsersByFirstAndOrLastName(nameOrEmail));
            }
        }

        // GET: api/users/class/n
        [HttpGet("class/{classId:guid}")]
        public ActionResult<IEnumerable<IdentityUser<Guid>>> GetUsersByClassId(Guid classId)
        {
            return new ActionResult<IEnumerable<IdentityUser<Guid>>>(this.identityUserRepository.FindUsersByClass(classId, this.enrolementRepository));
        }

        // PUT: api/users/n
        [HttpPut("{id:guid}")]
        public ActionResult PutUser(Guid id, IdentityUser<Guid> user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }
            this.identityUserRepository.UpdateUser(user);
            return NoContent();
        }

        // DELETE: api/users/n
        [HttpDelete("{id:guid}")]
        public ActionResult<IdentityUser<Guid>> DeleteUser(Guid id)
        {
            var user = this.identityUserRepository.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            this.identityUserRepository.DeleteUser(id);
            return user;
        }
    }
}