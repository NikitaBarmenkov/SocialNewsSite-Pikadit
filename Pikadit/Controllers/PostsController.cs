using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pikadit.DAL;
using Pikadit.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Pikadit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository postRepository;
        private Logger log;
        public PostsController(IPostRepository postrep)
        {
            postRepository = postrep;
        }
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<Post> GetAll()
        {
            return postRepository.GetPosts();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var post = postRepository.GetPost(id);//получить пост из бд по id
            if (post == null)//если не найден вернуть ошибку
            {
                return NotFound();
            }
            return Ok(post);
        }

        // POST api/<controller>
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody]Post post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            postRepository.CreatePost(post);//добавление поста
            try
            {
                postRepository.Save();//сохранение в бд
            }
            catch (Exception ex)
            {
                log.Write(ex);
            }
            return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }

        // PUT api/<controller>/5
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody]Post post)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Post item = postRepository.GetPost(id);//найти пост в бд по id
            if (item == null)
            {
                return NotFound();
            }
            item.Textbody = post.Textbody;
            item.Headline = post.Headline;
            postRepository.UpdatePost(item);
            try
            {
                postRepository.Save();//сохранение в бд
            }
            catch (Exception ex)
            {
                log.Write(ex);
            }
            return NoContent();
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        //[Route("api/posts/{PostId:int}/comments/{CommentId:int}")]
        //public async Task<IActionResult> Delete([FromRoute] int id, int PostId, int CommentId)
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Post item = postRepository.GetPost(id);
            if (item == null)
            {
                return NotFound();
            }
            postRepository.DeletePost(item.Id);
            try
            {
                postRepository.Save();//сохранение в бд
            }
            catch (Exception ex)
            {
                log.Write(ex);
            }
            return NoContent();
        }
    }
}
