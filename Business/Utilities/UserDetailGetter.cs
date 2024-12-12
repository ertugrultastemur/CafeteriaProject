using Core.Utilities.IoC;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Core.Entities.Concrete;
using System.Security.Claims;
using Core.Extensions;
using DataAccess.Abstract;
using Business.Abstract;
using Core.Dtos;

namespace Business.Utilities
{
    public class UserDetailGetter
    {
        private static IHttpContextAccessor _httpContextAccessor;
        private static UserResponseDto User;
        private static IUserService _userService;

        public UserDetailGetter(IUserService userService)
        {
            _userService = userService;
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
        }

        public static UserResponseDto GetDetails()
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            User = _userService.GetAll().Data.FirstOrDefault(u => u.Email==email);
            return User;
        }
    }
}
