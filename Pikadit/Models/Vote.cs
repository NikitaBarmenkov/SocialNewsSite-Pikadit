using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pikadit.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public int ForOrAgainst { get; set; }
        public int UserId { get; set; }
        public int? PostId { get; set; }
        public int? CommentId { get; set; }
        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
        public virtual Comment Comment { get; set; }
    }
}
