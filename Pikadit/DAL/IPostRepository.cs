using Pikadit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pikadit.DAL
{
    public interface IPostRepository : IDisposable
    {
        IEnumerable<Post> GetPosts();
        IEnumerable<Post> GetPostsForUser(int userid);
        Post GetPost(int postId);
        void CreatePost(Post post);
        void DeletePost(int postId);
        void UpdatePost(Post post);
        void Save();
    }
}
