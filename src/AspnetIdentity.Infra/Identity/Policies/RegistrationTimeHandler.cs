using Microsoft.AspNetCore.Authorization;

namespace AspnetIdentity.Infra.Identity.Policies
{
    public class RegistrationTimeHandler : AuthorizationHandler<RegistrationTimeRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        RegistrationTimeRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == "RegisteredIn"))
            {
                var data = context.User.FindFirst(c => c.Type == "RegisteredIn").Value;

                var dataRegistration = DateTime.Parse(data);

                double timeRegistration = await Task.Run(() =>
                                (DateTime.Now.Date - dataRegistration.Date).TotalDays);

                var timeInYears = timeRegistration / 360;

                if (timeInYears >= requirement.RegistrationTimeMin)
                {
                    context.Succeed(requirement);
                }
                return;
            }
        }
    }
}
