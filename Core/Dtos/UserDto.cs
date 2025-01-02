using Core.Entities.Abstract;
using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class UserDto : IDto
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public Byte[] PasswordHash { get; set; }

        public Byte[] PasswordSalt { get; set; }

        public int DepartmentId { get; set; }

        public List<int> OperationClaimIds { get; set; }

        public decimal Balance { get; set; }


        public static UserDto Generate(User user)
        {
            return new UserDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt,
                DepartmentId = user.DepartmentId,
                OperationClaimIds = user.OperationClaims.Select(op => op.OperationClaimId).ToList(),
                Balance = user.Balance
            };
        }

        public static User Generate(UserDto userDto)
        {
            return new User
            {
                Id = userDto.UserId,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = userDto.PasswordHash,
                PasswordSalt = userDto.PasswordSalt,
                DepartmentId = userDto.DepartmentId,
                Balance = userDto.Balance,
                IsDeleted = false
            };
        }
    }
}
