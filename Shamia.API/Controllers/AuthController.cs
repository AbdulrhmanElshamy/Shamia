using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Shamia.API.Dtos.Request;
using Shamia.API.Dtos.Response;
using Shamia.API.Services.InterFaces;
using Shamia.DataAccessLayer.Entities;

namespace Shamia.API.Controllers
{
    /// <summary>
    /// Handles user authentication, registration, and account management operations
    /// </summary>
    public class AuthController : BaseController
    {
        private readonly IIdentityService _identityService;
        private readonly UserManager<User> _userManager;
        public AuthController(IIdentityService identityService, UserManager<User> userManager)
        {
            _identityService = identityService;
            _userManager = userManager;
        }

        /// <summary>
        /// Registers a new user in the system
        /// </summary>
        /// <param name="userToCreate">User registration data</param>
        /// <returns>Success status and error messages if any</returns>
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser([FromBody] CreateNewUserRequest userToCreate)
        {
            if (!ModelState.IsValid)
                return BadRequest("");

            (bool IsSuccess, IEnumerable<string> Errors) = await _identityService.RegisterUser(userToCreate);
            if (Errors.Any() || !IsSuccess)
                return BadRequest(Errors);

            return Ok("User Created Successfully");
        }

        /// <summary>
        /// Confirms user email using verification token
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="token">Confirmation token sent via email</param>
        /// <returns>Confirmation result</returns>
        [HttpPost]
        [Route("email-confirm")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            var (IsSuccess, ErrorMessages) = await _identityService.ConfirmMail(email, token);
            if (!IsSuccess)
                return BadRequest(ErrorMessages);
            return Ok("Confiremd Successfully");
        }

        /// <summary>
        /// Initiates password reset process
        /// </summary>
        /// <param name="request">Password reset request details</param>
        /// <returns>Password reset initiation result</returns>
        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgotPasswordRequest request)
        {

            var (IsSuccess, ErrorMessages) = await _identityService.ForgortPassword(request);
            if (!IsSuccess)
                return BadRequest(ErrorMessages);

            return Ok("Check Your Email Please");
        }


        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {

            var (IsSuccess, ErrorMessages) = await _identityService.ResetPassword(request);
            if (!IsSuccess)
                return BadRequest(ErrorMessages);

            return Ok("Password Reset Successfully");
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LogIn([FromBody] UserLoginRequest userData)
        {
            if (!ModelState.IsValid)
                return BadRequest("");

            var loginResult = await _identityService.LogIn(userData);

            if (!loginResult.IsSuccess)
            {
                return BadRequest(loginResult.ErrorMessages);
            }
            var user = await _userManager.FindByEmailAsync(userData.Email);

            SetRefreshToken(loginResult.RefreshToken!);
            return Ok(new
            {
                token = loginResult.AccessToken,
                user = new
                {
                    Id = user.Id,
                    Email = user.Email,
                    User_Name = user.UserName,
                    Phone_Number = user.PhoneNumber,
                    Role = user.Role
                }

            });

        }

        [HttpGet]
        [Route("Refresh")]
        public async Task<IActionResult> RefreshToken([FromHeader] string authorization)
        {
            string refreshTokenVal = Request.Cookies.FirstOrDefault(x => x.Key == "refreshToken").Value;
            authorization = authorization.Split(" ")[1];

            var (IsSuccess, ErrorMessages, AccessToken) = await _identityService.RefreshToken(refreshTokenVal, authorization);

            if (!IsSuccess)
                return Unauthorized(ErrorMessages);

            return Ok(AccessToken);
        }

        [HttpPost]
        [Route("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var googleAuthResult = await _identityService.LogInWithGoogle(request.Token);

            if (!googleAuthResult.IsSuccess)
                return BadRequest(googleAuthResult.ErrorMessages);

            SetRefreshToken(googleAuthResult.RefreshToken!);

            return Ok(new
            {
                token = googleAuthResult.AccessToken,
                user = new
                {
                    Id = googleAuthResult.Response.Id,
                    Email = googleAuthResult.Response.Email,
                    User_Name = googleAuthResult.Response.User_Name,
                    Phone_Number = googleAuthResult.Response.Phone_Number,
                    Role = googleAuthResult.Response.Role
                }

            });
        }

        private void SetRefreshToken(RefreshToken refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.ExpiresOn,
                Secure = true,
                Path = "/"
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
            //Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5173/");

        }
    }
}
