using Google.Apis.Auth;

namespace Shamia.API.Services.InterFaces
{
    public interface IGoogleAuthService
    {
        public Task<GoogleJsonWebSignature.Payload> GetGoogleUserPayload(string token);

    }
}
