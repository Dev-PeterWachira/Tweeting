using Microsoft.AspNetCore.Authorization;

namespace Authorization
{
    public class WorksForCompanyRequirement : IAuthorizationRequirement
    {
        public string DomainName {get;} = string.Empty;
    

    public WorksForCompanyRequirement(string domainName)
        {
            DomainName = domainName;
        }
}
}
