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
using Entity.Concrete;
using Entity.Dtos;
using FluentValidation;

namespace Business.Utilities
{
    public static class UserDetailGetter
    {
        private static IHttpContextAccessor _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();


        public static UserDetailDto GetDetails()
        {
            IHttpContextAccessor httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();

            var claims = _httpContextAccessor.HttpContext.User.Identities.First().Claims;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value.Split(" ");
            return new UserDetailDto { 
                Id = Convert.ToInt32(claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value),
                FirstName = name[0],
                LastName = name[1],
                Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value,
                Roles = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
            };
        }
    }
}
