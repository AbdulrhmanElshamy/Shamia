using Shamia.Api.Dtos.Response;
using Shamia.API.Dtos.Request;
using Shamia.DataAccessLayer.Entities;
using System.Security.Claims;

namespace Shamia.API.Services.InterFaces
{
    public interface IIdentityService
    {
        public Task<(bool IsSuccess,IEnumerable<string> ErrorMessages)> RegisterUser(CreateNewUserRequest userToCreate);

        public Task<(bool IsSuccess,IEnumerable<string>? ErrorMessages)> ConfirmMail(string user, string token);
        public Task<(bool IsSuccess, IEnumerable<string>? ErrorMessages)> ResetPassword(ResetPasswordRequest requestData);

        public Task<(bool IsSuccess, IEnumerable<string>? ErrorMessages)> ForgortPassword(ForgotPasswordRequest request);
        public Task<(bool IsSuccess, IEnumerable<string>? ErrorMessages,string? AccessToken, RefreshToken? RefreshToken )> LogIn(UserLoginRequest userToCreate);

        public Task<
                         (bool IsSuccess,
                         IEnumerable<string>? ErrorMessages,
                         LoginWithGoogleResponse Response,
                         RefreshToken? RefreshToken,
                         string AccessToken)>
                 LogInWithGoogle(string token);
        public Task<(bool IsSuccess, IEnumerable<string>? ErrorMessages, string? NewAccessToken)> RefreshToken (string refreshToken, string accessToken);







    }
}
