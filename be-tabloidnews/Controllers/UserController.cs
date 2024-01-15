using System.Security.Claims;
using be_tabloidnews.DTOs;
using be_tabloidnews.Models;
using be_tabloidnews.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace be_tabloidnews.Controllers

{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        // GET: api/users
        [HttpGet]
        public ActionResult<List<User>> GetAllUser()
        {
            return userService.GetAllUsers();
        }

        // POST api/users
        [HttpPost]
        public ActionResult<User> CreateUser([FromBody] UserDTO user)
        {
            if (userService.CreateUser(user) != null)
            {
                return Ok("Tạo người dùng thành công.");
            }
            return BadRequest("Không thể tạo người dùng. Kiểm tra thông tin đầu vào hoặc người đã tồn tại.");
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(string id)
        {
            var user = userService.GetUserById(id);

            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng!");
            }

            return Ok(user);
        }

        // PUT api/users/5
        [HttpPut("{id}")]
        public ActionResult<User> UpdateUser(string id, [FromBody] User user)
        {
            if (user == null || !ModelState.IsValid)
            {
                return BadRequest("Thông tin không hợp lệ!");
            }

            var existingUser = userService.GetUserById(id);
            if (existingUser == null)
            {
                return NotFound("Không tìm thấy người dùng!");
            }

            userService.UpdateUser(id, user);

            return Ok(user);
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(string id)
        {
            try
            {
                var user = userService.GetUserById(id);
                if (user == null)
                {
                    return NotFound();
                }
                userService.DeleteUser(id);
                return Ok("Xóa người dùng thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        // POST api/users/login
        [HttpPost]
        [Route("login")]
        public ActionResult<User> Login([FromBody] LoginData data)
        {
            try
            {

                var token = userService.AuthenticateUser(data.username, data.password);
                if (token.IsNullOrEmpty())
                {
                    return BadRequest("Tên đăng nhập hoặc mật khẩu không hợp lệ!");
                }
                else
                {
                    return Ok(new { token = token, message = "Đăng nhập thành công!" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        // GET api/users/checktoken
        [HttpPost]
        [Route("checktoken")]
        public ActionResult<object> CheckTokenValidity([FromBody] TokenRequest tokenRequest)
        {
            bool valid = userService.TokenValidation(tokenRequest.Token, out ClaimsPrincipal principal);

            if (valid)
            {
                User userInfo = GetUserFromClaims(principal);
                return StatusCode(200, new { valid = true, displayName = userInfo?.displayName, message = "Token hợp lệ" });
            }
            else
            {
                return StatusCode(404, new { valid = false, message = "Token đã hết hạn hoặc không hợp lệ" });
            }
        }

        [HttpPost]
        [Route("checkadmin")]
        public ActionResult<bool> CheckAdmin([FromBody] TokenRequest tokenRequest)
        {
            bool valid = userService.TokenValidation(tokenRequest.Token, out ClaimsPrincipal principal);

            if (valid)
            {
                // Kiểm tra xem người dùng có vai trò "admin" hay không
                var roles = principal.FindAll(ClaimTypes.Name);
                bool isAdmin = roles.Any(r => r.Value == "admin");

                if (isAdmin)
                {
                    return StatusCode(200, new { isAdmin = true, message = "Người dùng là admin" });
                }
                else
                {
                    return StatusCode(403, new { isAdmin = false, message = "Người dùng không có quyền admin" });
                }
            }
            else
            {
                return StatusCode(404, new { valid = false, message = "Token đã hết hạn hoặc không hợp lệ" });
            }
        }


        private User GetUserFromClaims(ClaimsPrincipal principal)
        {
            var user = new User();

            foreach (var claim in principal.Claims)
            {
                switch (claim.Type)
                {
                    case ClaimTypes.GivenName:
                        user.displayName = claim.Value;
                        break;
                    case ClaimTypes.Name:
                        user.roleId = claim.Value;
                        break;
                    default:
                        break;
                }
            }
            return user;
        }
    }
}
