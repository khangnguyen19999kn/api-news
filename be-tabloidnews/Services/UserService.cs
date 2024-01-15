using be_tabloidnews.DTOs;
using be_tabloidnews.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace be_tabloidnews.Services
{
    public class UserService : IUserService
    {

        private readonly IMongoCollection<User> _users;

        public UserService(IUserDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>(settings.UserConnectionName);
        }

        public List<User> GetAllUsers()
        {
            return _users.Find(u => true).ToList();
        }


        public User CreateUser(UserDTO user)
        {
            // Kiểm tra user có tồn tại hay không
            var existingUser = _users.Find(u => u.username == user.username).FirstOrDefault();
            if (existingUser != null)
            {
                throw new Exception($"User already exits");
            }
            else
            {
                // Tạo mới đối tượng User từ UserDTO
                var newUser = new User
                {
                    username = user.username,
                    password = EncryptPassword(user.password),
                    displayName = user.displayName,
                    roleId = user.roleId
                };

                // Insert vào CSDL
                _users.InsertOne(newUser);

                // Trả về đối tượng User đã thêm vào CSDL
                return newUser;
            }
        }


        public void DeleteUser(string id)
        {
            _users.DeleteOne(u => u.id == id);
        }

        public User GetUserById(string id)
        {
            return _users.Find(u => u.id == id).FirstOrDefault();
        }

        public void UpdateUser(string id, User user)
        {
            user.password = EncryptPassword(user.password);
            _users.ReplaceOne(u => u.id == id, user);
        }

        // Phương thức xác thực user bằng username và password
        public string AuthenticateUser(string username, string password)
        {
            // Kiểm tra username và password có hợp lệ hay không
            var user = _users.Find(u => u.username == username).FirstOrDefault();
            if (user != null && VerifyPassword(password, user.password))
            {
                // Tạo token
                string tokenString = GenerateToken(user);
                return tokenString;
            }
            else
            {
                return null;
            }
        }

        // Phương thức phân quyền user bằng roleId
        public bool AuthorizeUser(string id, string roleId)
        {
            var user = _users.Find(u => u.id == id && u.roleId == roleId).FirstOrDefault();
            if (user != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string EncryptPassword(string password)
        {
            // Mã hóa password bằng BCrypt
            int cost = 10; // Độ phức tạp của thuật toán, càng cao càng an toàn nhưng càng chậm
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password, cost);

            return passwordHash;
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // Xác minh password bằng BCrypt
            bool verified = BCrypt.Net.BCrypt.Verify(password, passwordHash);

            // Trả về kết quả xác minh
            return verified;
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("this is my custom Secret key for authentication"); // Secret key để mã hóa và giải mã token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                        new Claim(ClaimTypes.GivenName, user.displayName),
                        new Claim(ClaimTypes.Name, user.roleId)
                }),
                Expires = DateTime.UtcNow.AddMonths(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) // Cách mã hóa token
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public bool TokenValidation(string token, out ClaimsPrincipal principal)
        {
            principal = null;

            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("this is my custom Secret key for authentication");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            try
            {
                principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                return validatedToken.ValidTo > DateTime.UtcNow;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
    }
}
