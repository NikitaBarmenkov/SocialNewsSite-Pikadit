using Pikadit.DAL;
using Pikadit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pikadit.BLL
{
    public class KarmaService
    {
        private IPostRepository postRepository;
        private IUserRepository userRepository;
        List<Post> posts;
        KarmaService(IPostRepository postrep, IUserRepository userrep)
        {
            postRepository = postrep;
            userRepository = userrep;
        }
        public void SetKarma(int userid)
        {
            int karma = 0;
            posts = postRepository.GetPostsForUser(userid).ToList();
            foreach(Post p in posts)
            {
                karma += p.Rating;
            }
            User user = userRepository.GetUser(userid);
            user.Karma = karma;
            userRepository.UpdateUser(user);
            userRepository.Save();
        }
    }
}
