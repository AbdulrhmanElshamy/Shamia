using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Org.BouncyCastle.Asn1.Ocsp;
using Shamia.Api.Dtos.Response;
using Shamia.API.Common;
using Shamia.API.Dtos.Request;
using Shamia.API.Services.InterFaces;
using Shamia.DataAccessLayer;
using Shamia.DataAccessLayer.Entities;
using System.Net;
using System.Security.Claims;

namespace Shamia.API.Services
{
    /// <summary>
    /// Manages user authentication, authorization, and identity operations
    /// </summary>
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IJwtHandlerService _jwtHandlerService;
        private readonly ShamiaDbContext _context;

        public IdentityService(
            UserManager<User> userManager,
            IEmailService emailService,
            IGoogleAuthService googleAuthService,
            IJwtHandlerService jwtHandlerService,
            ShamiaDbContext context)
        {
            _userManager = userManager;
            _emailService = emailService;
            _googleAuthService = googleAuthService;
            _jwtHandlerService = jwtHandlerService;
            _context = context;
        }

        /// <summary>
        /// Authenticates a user using email and password credentials
        /// </summary>
        /// <param name="user">Login credentials DTO</param>
        /// <returns>Tuple containing auth tokens, success status, and error messages</returns>
        public async Task<(bool IsSuccess,
                    IEnumerable<string>? ErrorMessages,
                    string? AccessToken,
                    RefreshToken? RefreshToken)>
                    LogIn(UserLoginRequest user)
        {
            var foundUser = await _userManager.FindByEmailAsync(user.Email);

            if (foundUser is null || !await _userManager.CheckPasswordAsync(foundUser, user.Password))
                return (false, ["Email Or Password Are Invalid"], null, null);

            Console.WriteLine(foundUser);
            if (!foundUser.EmailConfirmed)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(foundUser);
                var param = new Dictionary<string, string>
                {
                    {"token" , token },
                    {"email", user.Email!}
                };

                var callback = QueryHelpers.AddQueryString(user.ClientUri, param);
                string emailContent = $@"
                <html>
                <body style='font-family: Arial, sans-serif; text-align: center; background-color: #f4f4f4; padding: 20px;'>
                    <div style='max-width: 500px; background: white; padding: 20px; border-radius: 10px; box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);'>
                        <h2 style='color: #333;'>Confirm Your Email</h2>
                        <p style='color: #555; font-size: 16px;'>Thank you for signing up! Please confirm your email by clicking the button below:</p>
                        <a href='{callback}' style='background-color: #008CBA; color: white; padding: 14px 24px; text-decoration: none; font-size: 16px; border-radius: 5px; display: inline-block; margin-top: 20px;'>
                            Confirm Email
                        </a>
                        <p style='color: #777; font-size: 14px; margin-top: 20px;'>If you did not request this, please ignore this email.</p>
                    </div>
                </body>
                </html>";
                var message = new Message(foundUser.UserName, user.Email, "Email Confirmation Token", emailContent, isHtml: true);

                await _emailService.SendEmail(message);
                return (false, ["Please Verify Your Email"], null, null);

            }

            var generatedTokens = await _jwtHandlerService.HandleJwtTokensCreation(foundUser);

            if (!generatedTokens.IsSuccess)
                return (false, generatedTokens.ErrorMessages, null, null);

            return (true, null, generatedTokens.AccessToken, generatedTokens.RefreshToken);
        }

        /// <summary>
        /// Registers a new user in the system and sends confirmation email
        /// </summary>
        /// <param name="userToCreate">User registration data DTO</param>
        /// <returns>Tuple containing success status and error messages</returns>
        public async Task<(bool IsSuccess, IEnumerable<string> ErrorMessages)>
            RegisterUser(CreateNewUserRequest userToCreate)
        {
            var user = new User
            {
                UserName = userToCreate.UserName,
                Email = userToCreate.Email,
                City = userToCreate.City,
                Role = userToCreate.Role ?? "customer",
                PhoneNumber = userToCreate.Phone
            };

            var result = await _userManager.CreateAsync(user, userToCreate.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return (false, errors);
            }



            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token); 

            var param = new Dictionary<string, string>
            {
                {"token" , encodedToken },
                {"email", user.Email!}
            };

            var callback = QueryHelpers.AddQueryString(userToCreate.ClientUri, param);
            var emailContent = $@"
                <html>
                <body style='font-family: Arial, sans-serif; text-align: center;'>
                    <h2>Email Confirmation</h2>
                    <p>Please confirm your email by clicking the button below:</p>
                    <a href='{callback}' style='background-color: #008CBA; color: white; padding: 12px 20px; text-decoration: none; font-size: 16px; border-radius: 5px; display: inline-block;'>Confirm Email</a>
                    <p>If you did not request this, please ignore this email.</p>
                </body>
                </html>";
            var message = new Message(user.UserName, user.Email, "Email Confirmation Token", emailContent, isHtml: true);
            await _emailService.SendEmail(message);

            return (true, Enumerable.Empty<string>());
        }

        public async Task<(bool IsSuccess,
                    IEnumerable<string>? ErrorMessages,
                    string? AccessToken)> HandleRefreshToken(string refresToken, string accessToken)
        {
            var refrreshResult = await _jwtHandlerService.VerifyRefreshAndGenerateAccessAsync(refresToken, accessToken);
            if (!refrreshResult.IsSuccess)
                return (false, refrreshResult.ErrorMessages, null);
            return (true, null, refrreshResult.AccessToken);

        }

        public async Task<(bool IsSuccess,
                     IEnumerable<string>? ErrorMessages)>
            ConfirmMail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, ["User Not Found"]);

            var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!confirmResult.Succeeded)
                return (false, ["Invalid Email Confirmation Request"]);
            return (true, null);
        }


        public async Task<
                    (bool IsSuccess,
                    IEnumerable<string>? ErrorMessages,
                    LoginWithGoogleResponse Response,
                    RefreshToken? RefreshToken,
                    string AccessToken)>
            LogInWithGoogle(string token)
        {
            var googleUser = await _googleAuthService.GetGoogleUserPayload(token);
            var exisitUser = await _userManager.FindByEmailAsync(googleUser.Email);


            Console.WriteLine("############################");

            if (exisitUser != null)
            {
                var tokens = await _jwtHandlerService.HandleJwtTokensCreation(exisitUser);

                if (!tokens.IsSuccess)
                    return (false, tokens.ErrorMessages, null, null, null);

                return (
                    true,
                    null,
                    new LoginWithGoogleResponse
                    {
                        User_Name = exisitUser.UserName,
                        Id = exisitUser.Id,
                        Email = exisitUser.Email,
                        Phone_Number = exisitUser.PhoneNumber,
                        Role = exisitUser.Role
                    },
                    tokens.RefreshToken,
                    tokens.AccessToken!);
            }

            var userToCreate = new User
            {
                UserName = googleUser.GivenName,
                Email = googleUser.Email,
                City = "Kuwait",
                Role = "customer",
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(userToCreate);

            if (!result.Succeeded)
                return (false, ["Could not create user"], null, null, null);

            var userInfo = new UserLoginInfo("google", googleUser.Subject, "GOOGLE");
            await _userManager.AddLoginAsync(userToCreate, userInfo);


            var generatedTokens = await _jwtHandlerService.HandleJwtTokensCreation(userToCreate);

            if (!generatedTokens.IsSuccess)
                return (false, generatedTokens.ErrorMessages, null, null, null);


            return (true, null,
                new LoginWithGoogleResponse
                {
                    User_Name = userToCreate.UserName,
                    Id = userToCreate.Id,
                    Email = userToCreate.Email,
                    Phone_Number = userToCreate.PhoneNumber,
                    Role = userToCreate.Role
                },
            generatedTokens.RefreshToken,
            generatedTokens.AccessToken!);
        }

        public async Task<(bool IsSuccess, IEnumerable<string>? ErrorMessages, string? NewAccessToken)> RefreshToken(string refreshToken, string accessToken)
        {
            var HandleRefreshTokenResult = await _jwtHandlerService.VerifyRefreshAndGenerateAccessAsync(refreshToken, accessToken);
            if (!HandleRefreshTokenResult.IsSuccess)
                return (false, HandleRefreshTokenResult.ErrorMessages, null);

            return (true, null, HandleRefreshTokenResult.AccessToken);
        }


        public async Task<(bool IsSuccess, IEnumerable<string>? ErrorMessages)> ForgortPassword(ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return (false, ["no such email"]);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var param = new Dictionary<string, string>
            {
                {"tokenReset" , token },
                {"emailReset", user.Email!}
            };

            var callback = QueryHelpers.AddQueryString(request.ClientUri, param);
            string emailContent = $@"
                <html>
                <body style='font-family: Arial, sans-serif; text-align: center; background-color: #f4f4f4; padding: 20px;'>
                    <div style='max-width: 500px; background: white; padding: 20px; border-radius: 10px; box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);'>
                        <h2 style='color: #333;'>Reset Your Password</h2>
                        <p style='color: #555; font-size: 16px;'>We received a request to reset your password. Click the button below to proceed:</p>
                        <a href='{callback}' style='background-color: #FF5733; color: white; padding: 14px 24px; text-decoration: none; font-size: 16px; border-radius: 5px; display: inline-block; margin-top: 20px;'>
                            Reset Password
                        </a>
                        <p style='color: #777; font-size: 14px; margin-top: 20px;'>If you did not request this, please ignore this email.</p>
                    </div>
                </body>
                </html>";

            var message = new Message(user.UserName, user.Email, "Reset Password Token", emailContent, isHtml: true);
            
            _emailService.SendEmail(message);
            return (true, Enumerable.Empty<string>());
        }

        public async Task<(bool IsSuccess, IEnumerable<string>? ErrorMessages)> ResetPassword(ResetPasswordRequest requestData)
        {
            var user = await _userManager.FindByEmailAsync(requestData.Email);
            if (user == null)
                return (false, ["no such email"]);

            var result = await _userManager.ResetPasswordAsync(user, requestData.Token, requestData.Password);

            if (!result.Succeeded)
                return (false, ["Reset Failed"]);

            return (true, null);
        }
    }
}
