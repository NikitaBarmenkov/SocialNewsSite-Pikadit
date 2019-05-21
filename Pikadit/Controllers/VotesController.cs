using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pikadit.Models;

namespace Pikadit.Controllers
{
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly SiteContext _context;
        public VotesController(SiteContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("api/[controller]")]
        public async Task<IActionResult> GetPost([FromBody] VoteViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Vote vote = _context.Votes.SingleOrDefault(v => v.PostId == vm.PostId & v.UserId == vm.UserId);
            if (vote == null)
            {
                return NotFound();
            }
            return Ok(vote);
        }

        [Route("api/PostVotes")]
        [HttpPost]
        public async Task<IActionResult> PostVotes([FromBody]VoteViewModel votevm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Vote vote = _context.Votes.SingleOrDefault(v => v.PostId == votevm.PostId & v.UserId == votevm.UserId);//нахождение голоса по id пользователя и id поста
            int rating = 0;
            if (vote == null)
            {
                rating = votevm.Rating;
                if (votevm.ForOrAgainst == 1)
                    rating++;
                else if (votevm.ForOrAgainst == 0)
                    rating--;
                //добавить голос
                vote = new Vote
                {
                    PostId = votevm.PostId,
                    UserId = votevm.UserId,
                    ForOrAgainst = votevm.ForOrAgainst
                };
                _context.Votes.Add(vote);
                await _context.SaveChangesAsync();
                //изменить пост
                var item = _context.Posts.Find(votevm.PostId);
                if (item == null)
                {
                    return NotFound();
                }
                item.Rating = rating;
                _context.Posts.Update(item);
                await _context.SaveChangesAsync();

                return Ok();
            }
            else
            {
                if (votevm.ForOrAgainst == vote.ForOrAgainst)
                {
                    rating = votevm.Rating;
                    if (votevm.ForOrAgainst == 1)
                        rating--;
                    else if (votevm.ForOrAgainst == 0)
                        rating++;
                    _context.Votes.Remove(vote);
                    await _context.SaveChangesAsync();
                    //изменить пост
                    var item = _context.Posts.Find(votevm.PostId);
                    if (item == null)
                    {
                        return NotFound();
                    }
                    item.Rating = rating;
                    _context.Posts.Update(item);
                    await _context.SaveChangesAsync();

                    return NoContent();
                    //удалить голос
                }
                else
                {
                    //изменить голос
                    rating = votevm.Rating;
                    if (votevm.ForOrAgainst == 1)
                        rating += 2;
                    else if (votevm.ForOrAgainst == 0)
                        rating -= 2;
                    vote.ForOrAgainst = votevm.ForOrAgainst;
                    _context.Votes.Update(vote);
                    await _context.SaveChangesAsync();
                    //изменить пост
                    var item = _context.Posts.Find(votevm.PostId);
                    if (item == null)
                    {
                        return NotFound();
                    }
                    item.Rating = rating;
                    _context.Posts.Update(item);
                    await _context.SaveChangesAsync();

                    return NoContent();
                }
            }
        }

        private void UpdatePostRating()
        {

        }
    }
}
