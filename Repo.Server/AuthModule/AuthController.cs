using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
public class AuthController : ControllerBase
{
   private readonly IAuthUserService _authService;
    
   public AuthController(IAuthUserService _authService)
   {
      this._authService = _authService;
   }
   
   [HttpPost( "register")]
   public async Task<IActionResult> Register(RegistrationModel model)
   {
      try{
     
      return Ok(new { Message = "Użytkownik został zarejestrowany pomyślnie" });
      }
      catch (Exception ex)
      {
         return StatusCode(500, new { Message = "Wystąpił błąd podczas rejestracji", Error = ex.Message });
      }
   }
    
    
}