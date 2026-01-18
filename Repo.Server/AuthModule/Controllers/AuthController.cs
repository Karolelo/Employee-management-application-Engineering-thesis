using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Core.Models.auth;
using Repo.Server.Controllers.Interfaces;

namespace Repo.Server.AuthModule.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
   private readonly IAuthUserService _authService;
    
   public AuthController(IAuthUserService authService)
   {
      this._authService = authService;
   }
   
   [HttpPost("register")]
   public async Task<IActionResult> Register([FromBody]RegistrationModel model)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      var response = await _authService.CreateUser(model);
    
      return response.Success
         ? Ok(new { Message = "User registered successfully" })
         : BadRequest(new { Message = response.Error });
   }
  
   [HttpPost("login")]
   public async Task<IActionResult> Login([FromBody]LoginModel model)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      var response = await _authService.Login(model);
    
      return response.Success
         ? Ok(new { Message = "User logged in successfully", Token = response.Data })
         : Unauthorized(new { Message = response.Error });
   }
   
   [HttpPost("refresh-token")]
   public async Task<IActionResult> RefreshToken(TokenModel model)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);
      
      var response = await _authService.RefreshToken(model);
      
      return response.Success
         ? Ok(new { Message = "Token refreshed successfully", Token = response.Data })
         : Unauthorized(new { Message = response.Error });

   }
}