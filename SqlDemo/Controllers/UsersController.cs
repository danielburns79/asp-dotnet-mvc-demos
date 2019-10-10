using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SqlDemo.Models;
using System.Linq;

namespace SqlDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IClassRepository classRepository;
        private readonly IEnrolementRepository enrolementRepository;

        //public UsersController(IUserRepository userRepository) => this.userRepository = userRepository;
        public UsersController(IUserRepository userRepository, IClassRepository classRepository, IEnrolementRepository enrolementRepository)
        {
            this.userRepository = userRepository;
            this.classRepository = classRepository;
            this.enrolementRepository = enrolementRepository;
        }

        // POST: api/users
        [HttpPost]
        public ActionResult<User> PostUser(User user)
        {
            this.userRepository.CreateUser(user);
            return CreatedAtAction("PostUser", new User{UserId = user.UserId}, user);
        }

        // GET: api/users
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return new ActionResult<IEnumerable<User>>(this.userRepository.GetUsers());
        }

        // GET: api/users/n
        [HttpGet("{id:guid}")]
        public ActionResult<User> GetUser(Guid id)
        {
            var user = this.userRepository.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        // GET: api/users/nameoremail
        [HttpGet("{nameOrEmail}")]
        public ActionResult<IEnumerable<User>> GetUsers(string nameOrEmail)
        {
            if (nameOrEmail.Contains("@"))
            {
                return new ActionResult<IEnumerable<User>>(this.userRepository.FindUsersByEmail(nameOrEmail));
            }
            else
            {
                return new ActionResult<IEnumerable<User>>(this.userRepository.FindUsersByFirstAndOrLastName(nameOrEmail));
            }
        }

        // GET: api/users/class/n
        [HttpGet("class/{classId:guid}")]
        public ActionResult<IEnumerable<User>> GetUsersByClassId(Guid classId)
        {
            return new ActionResult<IEnumerable<User>>(this.userRepository.FindUsersByClass(classId, this.enrolementRepository));
        }

        // PUT: api/users/n
        [HttpPut("{id:guid}")]
        public ActionResult PutUser(Guid id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }
            this.userRepository.UpdateUser(user);
            return NoContent();
        }

        // DELETE: api/users/n
        [HttpDelete("{id:guid}")]
        public ActionResult<User> DeleteUser(Guid id)
        {
            var user = this.userRepository.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            this.userRepository.DeleteUser(id);
            return user;
        }
    }
}