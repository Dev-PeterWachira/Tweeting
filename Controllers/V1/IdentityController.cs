using Contracts.V1;
using Microsoft.AspNetCore.Mvc;
using Tweeting_book.Services;
using System;

public class IdentityController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public IdentityController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost(ApiRoutes.Identity.Register)]
    
    public async Task<IActionResult> Register([FromBody]UserRegistrationRequest request)
    {
        return Ok();
    }

    

}