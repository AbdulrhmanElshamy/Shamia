using Shamia.API.Common;

namespace Shamia.API.Services.InterFaces
{
    public interface IEmailService
    {
        public Task   SendEmail(Message email);
    }
}
