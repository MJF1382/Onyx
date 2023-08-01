using Onyx.Classes;

namespace Onyx.Models.ViewModels
{
    public class UserClaimsViewModel
    {
        public string UserId { get; set; }
        public List<UserClaim> Claims { get; set; }
    }
}
