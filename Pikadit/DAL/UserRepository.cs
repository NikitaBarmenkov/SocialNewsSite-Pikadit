using Microsoft.EntityFrameworkCore;
using Pikadit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pikadit.DAL
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private SiteContext context;

        public UserRepository(SiteContext context)
        {
            this.context = context;
        }

        public IEnumerable<User> GetUsers()
        {
            return context.Users.ToList();
        }

        public User Login(string email, string password)
        {
            return context.Users.SingleOrDefault(m => m.Email == email & m.Password == password);
        }

        public User GetUser(int id)
        {
            return context.Users.Find(id);
        }

        public void CreateUser(User user)
        {
            user.Role = "user";
            context.Users.Add(user);
        }

        public void DeleteUser(int userId)
        {
            User user = context.Users.Find(userId);
            context.Users.Remove(user);
        }

        public void UpdateUser(User user)
        {
            context.Entry(user).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void SetKarma(int userid)
        {
            List<Post> posts;
            int karma = 0;
            posts = context.Posts.Where(p => p.UserId == userid).ToList();
            foreach (Post p in posts)
            {
                karma += p.Rating;
            }
            User user = GetUser(userid);
            user.Karma = karma;
            UpdateUser(user);
            Save();
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
