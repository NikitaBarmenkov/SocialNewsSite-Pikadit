using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pikadit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pikadit.Controllers
{
    [Route("api/[controller]")]
    public class CommentsController : Controller
    {
        private readonly SiteContext _context;
        public CommentsController(SiteContext context)
        {
            _context = context;
        }
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<Comment> GetAll()
        {
            return _context.Comments;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var comment = await _context.Comments.SingleOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody]Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            comment.Author = _context.Users.Where(u => u.Id == comment.UserId).FirstOrDefault().UserName;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody]Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var item = _context.Comments.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            item.Text = comment.Text;
            item.PostId = comment.PostId;
            item.UserId = comment.UserId;
            item.Author = _context.Users.Where(u => u.Id == comment.UserId).FirstOrDefault().UserName;
            _context.Comments.Update(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var item = _context.Comments.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            _context.Comments.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
