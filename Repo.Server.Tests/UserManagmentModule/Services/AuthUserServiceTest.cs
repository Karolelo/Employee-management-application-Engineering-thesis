
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repo.Core.Infrastructure;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Infrastructure.UnityOfWork;
using Repo.Core.Models;
using Repo.Core.Models.auth;
using Repo.Server.AuthModule.Interfaces;
using Repo.Server.Controllers.Interfaces;
using Repo.Server.UserManagmentModule.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace Repo.Server.Tests.UserManagmentModule.Services;

[TestClass]
[TestSubject(typeof(AuthUserService))]
public class AuthUserServiceTest
{
    private AuthenticationHelpers _authenticationHelpers;
    private Mock<IUnityOfWork<MyDbContext>> _mockIUnityOfWork;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IRoleRepository> _mockRoleRepository;
    private Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
    private IAuthUserService _userService;

    [TestInitialize]
    public void SetUp()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(x => x["JWT:SecretKey"]).Returns("test-secret-key-that-is-very-long-for-testing-purposes-only");
        mockConfiguration.Setup(x => x["JWT:ValidIssuer"]).Returns("test-issuer");
        mockConfiguration.Setup(x => x["JWT:ValidAudience"]).Returns("test-audience");

        _authenticationHelpers = new AuthenticationHelpers(mockConfiguration.Object);
        _mockIUnityOfWork = new Mock<IUnityOfWork<MyDbContext>>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();
        _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();

        _userService = new AuthUserService(_authenticationHelpers
            , _mockIUnityOfWork.Object
            , _mockUserRepository.Object
            , _mockRoleRepository.Object
            , _mockRefreshTokenRepository.Object);
    }

    #region CreateUser Tests

    [TestMethod]
    public async Task CreateUser_ShouldReturnUser_WhenUserSuccessfullyCreated()
    {
        //Arrange
        var registrationModel = new RegistrationModel
        {
            Email = "exampleEmail@op.pl",
            Nickname = "krzesin",
            Login = "BigIdiot",
            Name = "Dawid",
            Surname = "Krzesinski",
            Password = "LubieBrzydkieKobiety",
            Role = null
        };
        var user = MapDtoToUser(registrationModel);
    
        _mockUserRepository.Setup(u => u.GetUserByEmail(registrationModel.Email))
            .ReturnsAsync(null as User);
        _mockUserRepository.Setup(u => u.GetUserByNickname(registrationModel.Nickname))
            .ReturnsAsync(null as User);
        _mockUserRepository.Setup(u => u.CreateUser(It.IsAny<User>()))
            .ReturnsAsync(user);
        _mockRoleRepository.Setup(r => r.GetAllRoles())
            .ReturnsAsync(new List<Role> 
            { 
                new Role { ID = 1, Role_Name = "User" } 
            });
        _mockUserRepository.Setup(u => u.UpdateUser(It.IsAny<User>()))
            .ReturnsAsync(user);
        _mockRefreshTokenRepository.Setup(r => r.Add(It.IsAny<RefreshToken>()))
            .ReturnsAsync(new RefreshToken());
        _mockIUnityOfWork.Setup(u => u.CreateTransaction()).Verifiable();
        _mockIUnityOfWork.Setup(u => u.Save()).Verifiable();
        _mockIUnityOfWork.Setup(u => u.Commit()).Verifiable();

        //Act
        var result = await _userService.CreateUser(registrationModel);

        //Assert
        result.Success.Should().BeTrue();
        result.Data.Email.Should().Be(registrationModel.Email);
        result.Data.Login.Should().Be(registrationModel.Login);
        result.Data.Nickname.Should().Be(registrationModel.Nickname);
        _mockIUnityOfWork.Verify(u => u.CreateTransaction(), Times.Once);
        _mockIUnityOfWork.Verify(u => u.Save(), Times.Once);
        _mockIUnityOfWork.Verify(u => u.Commit(), Times.Once);
        _mockRefreshTokenRepository.Verify(r => r.Add(It.IsAny<RefreshToken>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateUser_ShouldFail_WhenEmailAlreadyExists()
    {
        //Arrange
        var registrationModel = new RegistrationModel
        {
            Email = "existing@op.pl",
            Nickname = "newuser",
            Login = "newlogin",
            Name = "Jan",
            Surname = "Kowalski",
            Password = "SecurePassword123",
            Role = null
        };
        
        var existingUser = new User { ID = 1, Email = registrationModel.Email };
        
        _mockUserRepository.Setup(u => u.GetUserByEmail(registrationModel.Email))
            .ReturnsAsync(existingUser);
        _mockIUnityOfWork.Setup(u => u.CreateTransaction()).Verifiable();

        //Act
        var result = await _userService.CreateUser(registrationModel);

        //Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("User with this email already exists");
        _mockIUnityOfWork.Verify(u => u.Rollback(), Times.Never);
    }

    [TestMethod]
    public async Task CreateUser_ShouldFail_WhenNicknameAlreadyExists()
    {
        //Arrange
        var registrationModel = new RegistrationModel
        {
            Email = "newmail@op.pl",
            Nickname = "existingnickname",
            Login = "newlogin",
            Name = "Jan",
            Surname = "Kowalski",
            Password = "SecurePassword123",
            Role = null
        };
        
        var existingUser = new User { ID = 2, Nickname = registrationModel.Nickname };
        
        _mockUserRepository.Setup(u => u.GetUserByEmail(registrationModel.Email))
            .ReturnsAsync(null as User);
        _mockUserRepository.Setup(u => u.GetUserByNickname(registrationModel.Nickname))
            .ReturnsAsync(existingUser);
        _mockIUnityOfWork.Setup(u => u.CreateTransaction()).Verifiable();

        //Act
        var result = await _userService.CreateUser(registrationModel);

        //Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("User with this nickname already exists");
        _mockIUnityOfWork.Verify(u => u.Rollback(), Times.Never);
    }

    [TestMethod]
    public async Task CreateUser_ShouldFail_WhenExceptionOccurs()
    {
        //Arrange
        var registrationModel = new RegistrationModel
        {
            Email = "test@op.pl",
            Nickname = "testuser",
            Login = "testlogin",
            Name = "Test",
            Surname = "User",
            Password = "TestPassword123",
            Role = null
        };
        
        _mockUserRepository.Setup(u => u.GetUserByEmail(registrationModel.Email))
            .ReturnsAsync(null as User);
        _mockUserRepository.Setup(u => u.GetUserByNickname(registrationModel.Nickname))
            .ReturnsAsync(null as User);
        _mockUserRepository.Setup(u => u.CreateUser(It.IsAny<User>()))
            .ThrowsAsync(new Exception("Database error"));
        _mockIUnityOfWork.Setup(u => u.CreateTransaction()).Verifiable();
        _mockIUnityOfWork.Setup(u => u.Rollback()).Verifiable();

        //Act
        var result = await _userService.CreateUser(registrationModel);

        //Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Error during creating of user");
        _mockIUnityOfWork.Verify(u => u.Rollback(), Times.Once);
        _mockIUnityOfWork.Verify(u => u.Commit(), Times.Never);
    }

    [TestMethod]
    public async Task CreateUser_ShouldAssignDefaultRole_WhenNoRoleProvided()
    {
        //Arrange
        var registrationModel = new RegistrationModel
        {
            Email = "test@op.pl",
            Nickname = "testuser",
            Login = "testlogin",
            Name = "Test",
            Surname = "User",
            Password = "TestPassword123",
            Role = null
        };
        
        var user = MapDtoToUser(registrationModel);
        var defaultRole = new Role { ID = 1, Role_Name = "User" };
        
        _mockUserRepository.Setup(u => u.GetUserByEmail(registrationModel.Email))
            .ReturnsAsync(null as User);
        _mockUserRepository.Setup(u => u.GetUserByNickname(registrationModel.Nickname))
            .ReturnsAsync(null as User);
        _mockUserRepository.Setup(u => u.CreateUser(It.IsAny<User>()))
            .ReturnsAsync(user);
        _mockRoleRepository.Setup(r => r.GetAllRoles())
            .ReturnsAsync(new List<Role> { defaultRole });
        _mockUserRepository.Setup(u => u.UpdateUser(It.IsAny<User>()))
            .Callback<User>(u => u.Roles = new List<Role> { defaultRole })
            .ReturnsAsync(user);
        _mockRefreshTokenRepository.Setup(r => r.Add(It.IsAny<RefreshToken>()))
            .ReturnsAsync(new RefreshToken());
        _mockIUnityOfWork.Setup(u => u.CreateTransaction()).Verifiable();
        _mockIUnityOfWork.Setup(u => u.Save()).Verifiable();
        _mockIUnityOfWork.Setup(u => u.Commit()).Verifiable();

        //Act
        var result = await _userService.CreateUser(registrationModel);

        //Assert
        result.Success.Should().BeTrue();
        result.Data.Roles.Should().HaveCount(1);
        result.Data.Roles.First().Role_Name.Should().Be("User");
    }

    #endregion

    #region Login Tests

    [TestMethod]
    public async Task Login_ShouldReturnTokens_WhenCredentialsAreCorrect()
    {
        //Arrange
        var loginModel = new LoginModel
        {
            Login = "testlogin",
            Password = "TestPassword123"
        };
        
        var salt = AuthenticationHelpers.GenerateSalt(64);
        var user = new User
        {
            ID = 1,
            Login = loginModel.Login,
            Nickname = "testnickname",
            Email = "test@op.pl",
            Password = AuthenticationHelpers.GeneratePasswordHash(loginModel.Password, salt),
            Salt = salt,
            Roles = new List<Role> { new Role { Role_Name = "User" } }
        };
        
        var refreshToken = new RefreshToken
        {
            ID = 1,
            Token = "valid-refresh-token",
            User_ID = user.ID,
            CreatedAt = DateTime.Now.AddDays(-1),
            ExpireDate = DateTime.Now.AddDays(7),
            RevokedAt = null
        };
        
        _mockUserRepository.Setup(u => u.GetUserByLogin(loginModel.Login))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(u => u.GetUserRoles(user.ID))
            .ReturnsAsync(new List<string> { "User" });
        _mockRefreshTokenRepository.Setup(r => r.FindByUserId(user.ID))
            .ReturnsAsync(refreshToken);
        _mockUserRepository.Setup(u => u.GetUserByNickname(user.Nickname))
            .ReturnsAsync(user);
        _mockRefreshTokenRepository.Setup(r => r.FindByUserId(user.ID))
            .ReturnsAsync(refreshToken);

        //Act
        var result = await _userService.Login(loginModel);

        //Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.AccessToken.Should().NotBeNullOrEmpty();
        result.Data.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public async Task Login_ShouldFail_WhenUserDoesNotExist()
    {
        //Arrange
        var loginModel = new LoginModel
        {
            Login = "nonexistent",
            Password = "Password123"
        };
        
        _mockUserRepository.Setup(u => u.GetUserByLogin(loginModel.Login))
            .ReturnsAsync(null as User);

        //Act
        var result = await _userService.Login(loginModel);

        //Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("User with this login does not exist");
    }

    [TestMethod]
    public async Task Login_ShouldFail_WhenPasswordIsWrong()
    {
        //Arrange
        var loginModel = new LoginModel
        {
            Login = "testlogin",
            Password = "WrongPassword"
        };
        
        var salt = AuthenticationHelpers.GenerateSalt(64);
        var user = new User
        {
            ID = 1,
            Login = loginModel.Login,
            Nickname = "testnickname",
            Email = "test@op.pl",
            Password = AuthenticationHelpers.GeneratePasswordHash("CorrectPassword123", salt),
            Salt = salt,
            Roles = new List<Role>()
        };
        
        _mockUserRepository.Setup(u => u.GetUserByLogin(loginModel.Login))
            .ReturnsAsync(user);

        //Act
        var result = await _userService.Login(loginModel);

        //Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Wrong password");
    }

    [TestMethod]
    public async Task Login_ShouldCreateNewRefreshToken_WhenOldTokenIsInvalid()
    {
        //Arrange
        var loginModel = new LoginModel
        {
            Login = "testlogin",
            Password = "TestPassword123"
        };
        
        var salt = AuthenticationHelpers.GenerateSalt(64);
        var user = new User
        {
            ID = 1,
            Login = loginModel.Login,
            Nickname = "testnickname",
            Email = "test@op.pl",
            Password = AuthenticationHelpers.GeneratePasswordHash(loginModel.Password, salt),
            Salt = salt,
            Roles = new List<Role> { new Role { Role_Name = "User" } }
        };
        
        var expiredToken = new RefreshToken
        {
            ID = 1,
            Token = "expired-token",
            User_ID = user.ID,
            CreatedAt = DateTime.Now.AddDays(-10),
            ExpireDate = DateTime.Now.AddDays(-2),
            RevokedAt = null
        };
        
        _mockUserRepository.Setup(u => u.GetUserByLogin(loginModel.Login))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(u => u.GetUserRoles(user.ID))
            .ReturnsAsync(new List<string> { "User" });
        _mockRefreshTokenRepository.Setup(r => r.FindByUserId(user.ID))
            .ReturnsAsync(expiredToken);
        _mockRefreshTokenRepository.Setup(r => r.Update(It.IsAny<RefreshToken>()))
            .ReturnsAsync(expiredToken);
        _mockRefreshTokenRepository.Setup(r => r.Add(It.IsAny<RefreshToken>()))
            .ReturnsAsync(new RefreshToken { Token = "new-refresh-token" });

        //Act
        var result = await _userService.Login(loginModel);

        //Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        _mockRefreshTokenRepository.Verify(r => r.Update(It.IsAny<RefreshToken>()), Times.Once);
        _mockRefreshTokenRepository.Verify(r => r.Add(It.IsAny<RefreshToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Login_ShouldFail_WhenExceptionOccurs()
    {
        //Arrange
        var loginModel = new LoginModel
        {
            Login = "testlogin",
            Password = "TestPassword123"
        };
        
        _mockUserRepository.Setup(u => u.GetUserByLogin(loginModel.Login))
            .ThrowsAsync(new Exception("Database connection error"));

        //Act
        var result = await _userService.Login(loginModel);

        //Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Error during login");
    }

    #endregion

    #region RefreshToken Tests

    [TestMethod]
    public async Task RefreshToken_ShouldReturnNewAccessToken_WhenRefreshTokenIsValid()
    {
        //Arrange
        var tokenModel = new TokenModel
        {
            AccessToken = GenerateExpiredToken(1, "testuser"),
            RefreshToken = "valid-refresh-token"
        };
        
        var user = new User
        {
            ID = 1,
            Nickname = "testuser",
            Roles = new List<Role> { new Role { Role_Name = "User" } }
        };
        
        var validRefreshToken = new RefreshToken
        {
            ID = 1,
            Token = tokenModel.RefreshToken,
            User_ID = user.ID,
            CreatedAt = DateTime.Now.AddDays(-1),
            ExpireDate = DateTime.Now.AddDays(6),
            RevokedAt = null
        };
        
        _mockUserRepository.Setup(u => u.GetUserByNickname("testuser"))
            .ReturnsAsync(user);
        _mockRefreshTokenRepository.Setup(r => r.FindByUserId(user.ID))
            .ReturnsAsync(validRefreshToken);
        _mockUserRepository.Setup(u => u.GetUserRoles(user.ID))
            .ReturnsAsync(new List<string> { "User" });

        //Act
        var result = await _userService.RefreshToken(tokenModel);

        //Assert
        result.Success.Should().BeTrue();
        result.Data.AccessToken.Should().NotBeNullOrEmpty();
        result.Data.RefreshToken.Should().Be(tokenModel.RefreshToken);
    }

    [TestMethod]
    public async Task RefreshToken_ShouldFail_WhenAccessTokenIsInvalid()
    {
        //Arrange
        var tokenModel = new TokenModel
        {
            AccessToken = "invalid-token-format",
            RefreshToken = "valid-refresh-token"
        };

        //Act
        var result = await _userService.RefreshToken(tokenModel);

        //Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Bad token format");
    }

    [TestMethod]
    public async Task RefreshToken_ShouldFail_WhenRefreshTokenIsInvalid()
    {
        //Arrange
        var tokenModel = new TokenModel
        {
            AccessToken = GenerateExpiredToken(1, "testuser"),
            RefreshToken = "invalid-refresh-token"
        };
        
        var user = new User
        {
            ID = 1,
            Nickname = "testuser"
        };
        
        var validRefreshToken = new RefreshToken
        {
            ID = 1,
            Token = "different-token",
            User_ID = user.ID,
            CreatedAt = DateTime.Now.AddDays(-1),
            ExpireDate = DateTime.Now.AddDays(6),
            RevokedAt = null
        };
        
        _mockUserRepository.Setup(u => u.GetUserByNickname("testuser"))
            .ReturnsAsync(user);
        _mockRefreshTokenRepository.Setup(r => r.FindByUserId(user.ID))
            .ReturnsAsync(validRefreshToken);

        //Act
        var result = await _userService.RefreshToken(tokenModel);

        //Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Bad refresh token");
    }

    [TestMethod]
    public async Task RefreshToken_ShouldFail_WhenUserNotFound()
    {
        //Arrange
        var tokenModel = new TokenModel
        {
            AccessToken = GenerateExpiredToken(1, "nonexistent"),
            RefreshToken = "valid-refresh-token"
        };
        
        _mockUserRepository.Setup(u => u.GetUserByNickname("nonexistent"))
            .ReturnsAsync(null as User);

        //Act
        var result = await _userService.RefreshToken(tokenModel);

        //Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Bad refresh token");
    }

    #endregion

    #region Helper Methods

    private User MapDtoToUser(RegistrationModel model)
    {
        var salt = AuthenticationHelpers.GenerateSalt(64);
    
        var user = new User
        {
            Login = model.Login,
            Nickname = model.Nickname,
            Email = model.Email,
            Password = AuthenticationHelpers.GeneratePasswordHash(model.Password, salt),
            Salt = salt,
            Name = model.Name,
            Surname = model.Surname,
            Roles = new List<Role>()
        };
        return user;
    }

    private string GenerateExpiredToken(int userId, string userName)
    {
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x["JWT:SecretKey"]).Returns("test-secret-key-that-is-very-long-for-testing-purposes-only");
        mockConfig.Setup(x => x["JWT:ValidIssuer"]).Returns("test-issuer");
        mockConfig.Setup(x => x["JWT:ValidAudience"]).Returns("test-audience");
    
        var claims = new List<Claim>
        {
            new Claim("id", userId.ToString()),
            new Claim(ClaimTypes.Name, userName)
        };
    
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var key = System.Text.Encoding.ASCII.GetBytes("test-secret-key-that-is-very-long-for-testing-purposes-only");
    
        var now = DateTime.UtcNow;
        var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = "test-issuer",
            Audience = "test-audience",
            NotBefore = now.AddHours(-2), 
            IssuedAt = now.AddHours(-2),  
            Expires = now.AddHours(-1),   
            SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
        };
    
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    #endregion
}