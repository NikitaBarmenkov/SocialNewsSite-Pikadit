using Microsoft.EntityFrameworkCore;
using Pikadit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pikadit.DAL
{
    public class PostRepository : IPostRepository, IDisposable
    {
        private SiteContext context;
        List<Post> posts;

        public PostRepository(SiteContext context)
        {
            this.context = context;
        }

        public IEnumerable<Post> GetPosts()
        {
            posts = context.Posts.ToList();
            posts.Sort(delegate (Post p1, Post p2)
            { return p2.Rating.CompareTo(p1.Rating); });
            return posts;
        }

        public IEnumerable<Post> GetPostsForUser(int userid)
        {
            return context.Posts.Where(p => p.UserId == userid).ToList();
        }

        public Post GetPost(int id)
        {
            return context.Posts.Find(id);
        }

        public void CreatePost(Post post)
        {
            post.Author = context.Users.Where(u => u.Id == post.UserId).FirstOrDefault().UserName;//назаначение автора поста
            context.Posts.Add(post);
        }

        public void DeletePost(int postId)
        {
            Post post = context.Posts.Find(postId);
            context.Posts.Remove(post);
        }

        public void UpdatePost(Post post)
        {
            context.Entry(post).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
