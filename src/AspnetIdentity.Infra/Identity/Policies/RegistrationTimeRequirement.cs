using Microsoft.AspNetCore.Authorization;

namespace AspnetIdentity.Infra.Identity.Policies
{
    public class RegistrationTimeRequirement : IAuthorizationRequirement
    {
        public int RegistrationTimeMin { get; }

        public RegistrationTimeRequirement(int registrationTimeMin)
        {
            RegistrationTimeMin = registrationTimeMin;
        }
    }
}
