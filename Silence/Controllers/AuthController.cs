﻿using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Silence.Web.Entities;
using Silence.Web.Services;
using System.Threading.Tasks;
namespace Silence.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ConfigurationService _configurationService;
    private readonly DbService _db;

    public AuthController(
        AuthService authService,
        ConfigurationService configurationService,
        DbService db
        )
    {
        _authService = authService;
        _configurationService = configurationService;
        _db = db;
    }

    //Сделали ручку асинхронной
    [HttpPost("login")]
    async public Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
            var user = await _db.GetUser(request.UserName);
            if (user == null ||
                !_authService.VerifyPasswordHash(
                    request.Password, user.PasswordHash, user.Salt))
            {
                return Unauthorized("Invalid username or password.");
            }

            var accessToken = _authService.GenerateAccessToken(user);
            var refreshToken = _authService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                _configurationService.JwtRefreshTokenExpirationDays);
            //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
            await _db.SaveChanges();

            return Ok(new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id,
                Username = user.UserName
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.ReadKey();
        }
        return BadRequest();
    }

    //Сделали ручку асинхронной
    [HttpPost("refresh-token")]
    async public Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var principal = _authService.GetPrincipalFromExpiredToken(request.AccessToken);
        var username = principal.Identity.Name;
        //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
        var user = await _db.GetUser(username);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid token.");
        }

        var newAccessToken = _authService.GenerateAccessToken(user);
        var newRefreshToken = _authService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
        await _db.SaveChanges();

        return Ok(new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    //Сделали ручку асинхронной
    [HttpPost("register")]
    async public Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
        var existingUser = await _db.GetUser(request.Username);
        if (existingUser != null)
        {
            return BadRequest("Username already exists.");
        }
        _authService.CreatePasswordHash(request.Password,
        out string passwordHash, out string passwordSalt);

        var user = new User
        {
            UserName = request.Username,
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = passwordHash,
            Salt = passwordSalt
        };

        try
        {
            //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
            await _db.AddUser(user);
        }
        catch (Exception ex)
        {
            return BadRequest("Error registering user.");
        }

        return Ok("User registered successfully.");
    }

    //Сделали ручку асинхронной
    [Authorize]
    [HttpGet("validate-token")]
    async public Task<IActionResult> ValidateToken()
    {
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            var username = identity.FindFirst(ClaimTypes.Name)?.Value;
            //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
            var user = await _db.GetUser(username);

            if (user is null)
            {
                return Unauthorized();
            }

            return Ok(new ValidateTokenResponse()
            {
                UserId = user.Id,
                Username = user.UserName
            });
        }

        return Unauthorized();
    }

    //Сделали ручку асинхронной
    [Authorize]
    [HttpGet("logout")]
    async public Task<IActionResult> Logout()
    {
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            var username = identity.FindFirst(ClaimTypes.Name)?.Value;
            //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
            var user = await _db.GetUser(username);   

            if (user is null)
            {
                return Unauthorized();
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            //Взаимодействие с СУБД должно быть асинхронным, для 100% результата
            await _db.SaveChanges();

            return Ok();
        }

        return Unauthorized();
    }
}

