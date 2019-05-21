using Pikadit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pikadit.DAL
{
    public interface IUserRepository : IDisposable
    {
        IEnumerable<User> GetUsers();
        User GetUser(int userId);
        User Login(string email, string password);
        void CreateUser(User user);
        void DeleteUser(int userId);
        void UpdateUser(User user);
        void SetKarma(int userid);
        void Save();
    }
}
