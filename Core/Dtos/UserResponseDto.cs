using Core.Entities.Abstract;
using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class UserResponseDto : IDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }


        public static UserResponseDto Generate(User _user)
        {
            return new UserResponseDto
            {
                Id = _user.Id,
                FirstName = _user.FirstName,
                LastName = _user.LastName,
                Email = _user.Email,
            };
        }
    }
}
