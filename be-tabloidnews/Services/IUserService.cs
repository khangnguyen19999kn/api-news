using System.Security.Claims;
using be_tabloidnews.DTOs;
using be_tabloidnews.Models;

namespace be_tabloidnews.Services
{
    public interface IUserService
    {
        List<User> GetAllUsers();
        User GetUserById(string id);
        User CreateUser(UserDTO user);
        void UpdateUser(string id, User user);
        void DeleteUser(string id);
        string AuthenticateUser(string username, string password);
        bool AuthorizeUser(string id, string roleId);
        
        bool TokenValidation(string token, out ClaimsPrincipal principal);
    }
}
