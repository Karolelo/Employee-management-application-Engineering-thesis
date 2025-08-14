using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.auth;
using Repo.Core.Infrastructure;
using Repo.Server.Controllers.Interfaces;

namespace Repo.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
   private readonly IAuthUserService _authService;
    
   public AuthController(IAuthUserService _authService)
   {
      this._authService = _authService;
   }
   
   [HttpPost("register")]
   public async Task<IActionResult> Register(RegistrationModel model)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      var response = await _authService.CreateUser(model);
    
      return response.Success
         ? Ok(new { Message = "User registered successfully" })
         : BadRequest(new { Message = response.Error });
   }
  
   [HttpPost("login")]
   public async Task<IActionResult> Login(LoginModel model)
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