using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pikadit.BLL;
using Pikadit.DAL;
using Pikadit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pikadit.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private Logger log;
        public UsersController(IUserRepository userrep)
        {
            userRepository = userrep;
            //karmaService = ser;
        }
        //GET: api/<controller>
        [Route("api/[controller]")]
        [HttpGet]
        public IEnumerable<User> GetAll()
        {
            return userRepository.GetUsers();
        }

        [Route("api/Karma")]
        [HttpPost]
        public async Task<IActionResult> GetKarma([FromBody]User user)
        {
            var msg = new { message = "" };
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            userRepository.SetKarma(user.Id);
            user = userRepository.GetUser(user.Id);
            return Ok(user);
        }

        //GET api/<controller>/5
        [Route("api/Login")]
        [HttpPost]
        public async Task<IActionResult> GetUser([FromBody]User user)
        {
            var msg = new { message = "" };
            User checkuser;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (user.Email != "")
            {
                checkuser = userRepository.Login(user.Email, user.Password);
                if (checkuser == null)
                {
                    msg = new { message = "Такого пользователя нет" };
                    return NotFound(msg);
                }
                return Ok(checkuser);
            }
            else msg = new { message = "Вы не авторизовались" };
            return BadRequest(msg);
        }

        // POST api/<controller>
        [Route("api/Register")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody]User user)
        {
            var msg = new { message = "" };
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (user != null)
                if (user.Password.Length > 8)
                {
                    userRepository.CreateUser(user);
                    try
                    {
                        userRepository.Save();
                    }
                    catch (Exception ex)
                    {
                        log.Write(ex);
                    }
                    msg = new { message = "Добавлен пользователь: " + user.UserName };
                    return Ok(user);
                }
                else
                {
                    msg = new { message = "Пароль не соответствует требованиям" };
                    return BadRequest(msg);
                }
            else msg = new { message = "Поля не заполнены" };
            return BadRequest(msg);
            //return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // PUT api/<controller>/5
        [Route("api/[controller]")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody]User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User item = userRepository.GetUser(id);
            if (item == null)
            {
                return NotFound();
            }
            item.UserName = user.UserName;
            item.Email = user.Email;
            item.Id = user.Id;
            item.Password = user.Password;
            item.Role = user.Role;
            userRepository.UpdateUser(user);
            userRepository.Save();
            return NoContent();
        }

        // DELETE api/<controller>/5
        [Route("api/[controller]")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User item = userRepository.GetUser(id);
            if (item == null)
            {
                return NotFound();
            }
            userRepository.DeleteUser(item.Id);
            userRepository.Save();
            return NoContent();
        }
    }
}
