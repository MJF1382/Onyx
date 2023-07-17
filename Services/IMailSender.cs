using Onyx.Classes;

namespace Onyx.Services
{
    public interface IMailSender
    {
        Task SendAsync(Email email);
    }
}
