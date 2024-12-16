using Core.Entities.Abstract;
using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class UserRequestDto : IDto
    {
        public int Id { get; set; }

        public int DepartmentId { get; set; }

        public double Balance { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public List<int> OperationClaimIds { get; set; }



        public static UserRequestDto Generate(User _user)
        {
            return new UserRequestDto
            {
                Id = _user.Id,
                FirstName = _user.FirstName,
                LastName = _user.LastName,
                Balance = _user.Balance,
                Email = _user.Email,
                DepartmentId = _user.DepartmentId
            };
        }
    }
}
