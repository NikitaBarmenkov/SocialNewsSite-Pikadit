using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pikadit.Models
{
    public class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
            Votes = new HashSet<Vote>();
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Author { get; set; }
        public string Headline { get; set; }
        public string Textbody { get; set; }
        public int Rating { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }
        public virtual User User { get; set; }
    }
}
