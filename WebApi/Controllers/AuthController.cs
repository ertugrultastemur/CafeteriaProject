﻿using Business.Abstract;
using Core.Dtos;
using Core.Entities.Concrete;
using Core.Utilities.JWT;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public ActionResult Login(SignInDto signInDto)
        {
            var result = _authService.Login(signInDto);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("register")]
        public ActionResult Register(SignUpDto signUpDto)
        {
            var userExists = _authService.UserExists(signUpDto.Email);
            if (!userExists.IsSuccess)
            {
                return BadRequest(userExists.Message);
            }
            var result = _authService.Register(signUpDto);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("refresh")]
        public ActionResult Refresh([FromBody]TokenResponseDto login)
        {
            var result = _authService.Refresh(login);
            if (result == null || !result.IsSuccess)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }

            return Ok(result);
            
        }
    }
}