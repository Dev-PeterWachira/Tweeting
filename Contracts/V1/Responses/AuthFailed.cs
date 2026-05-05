using System;
using System.Collections.Generic;

namespace Contracts.V1.Responses
{
    public class AuthFailed
    {
        public IEnumerable<string> Errors {get; set;} = Array.Empty<string>();
    }
}

