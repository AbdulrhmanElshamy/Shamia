using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Shamia.API.Config;
using Shamia.API.Services.InterFaces;

namespace Shamia.API.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly GoogleAuthConfiguration _config;
        public GoogleAuthService(IOptions<GoogleAuthConfiguration> config)
        {
            _config = config.Value;
        }

        public async Task<GoogleJsonWebSignature.Payload> GetGoogleUserPayload(string token)
        {
            try
            {
                var googleUser = await GoogleJsonWebSignature.ValidateAsync(token,
                 new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { _config.ClientId }
                });
                    return googleUser;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error validating Google user: {ex.Message}");                
            }
          
        }
        
    }
}
