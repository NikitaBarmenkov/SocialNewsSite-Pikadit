using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pikadit.Models
{
    public class Comment
    {
        public Comment()
        {
            Votes = new HashSet<Vote>();
        }
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Author { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }
    }
}
