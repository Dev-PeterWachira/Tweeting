using Contracts.V1;
using Microsoft.AspNetCore.Mvc;
using Tweeting_book.Services;
using System;
using Contracts.V1.Responses;
using Contracts.V1.Requests;

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

        if (!ModelState.IsValid)
        {
           return BadRequest(new AuthFailed
           {
              Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx=>xx.ErrorMessage))
           });
        }

       var authResponse = await _identityService.RegisterAsync(request.Email, request.Password);

        if (!authResponse.Success)
        {
            return  BadRequest (new AuthFailed
            {
                Errors = authResponse.Errors
            }) ;
        }
        return Ok(new AuthSuccessResponse
        {
            Token = authResponse.Token
        });
    }



    [HttpPost(ApiRoutes.Identity.Login)]
    
    public async Task<IActionResult> Login([FromBody]UserLoginRequest request)
    {

       var authResponse = await _identityService.LoginAsync(request.Email, request.password);

        if (!authResponse.Success)
        {
            return  BadRequest (new AuthFailed
            {
                Errors = authResponse.Errors
            }) ;
        }
        return Ok(new AuthSuccessResponse
        {
            Token = authResponse.Token
        });
    }

    

}