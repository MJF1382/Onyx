using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Onyx.Models.Identity.Entities;
using System.Security.Claims;

namespace Onyx.Authorization.Policies
{
    public class ExternalLoginUsersHandler : AuthorizationHandler<ExternalLoginUsersRequirement>
    {
        private readonly UserManager<AppUser> _userManager;

        public ExternalLoginUsersHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExternalLoginUsersRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var user = _userManager.FindByIdAsync(_userManager.GetUserId(context.User)).Result;
                var userLogins = _userManager.GetLoginsAsync(user).Result.ToList();

                if (userLogins.Count > 0)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
