using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pikadit.Models
{
    public class VoteViewModel
    {
        public int ForOrAgainst { get; set; }
        public int UserId { get; set; }
        public int? PostId { get; set; }
        public int? CommentId { get; set; }
        public int Rating { get; set; }
    }
}
