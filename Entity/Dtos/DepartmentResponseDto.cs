using Core.Dtos;
using Core.Entities.Abstract;
using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class DepartmentResponseDto : IDto
    {
        public int DepartmentId { get; set; }
        public int BranchId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Floor { get; set; }
        public List<UserResponseDto> Users { get; set; }


        public static DepartmentResponseDto Generate(Department department)
        {
            return new DepartmentResponseDto
            {
                DepartmentId = department.Id,
                BranchId = department.Branch.Id,
                Name = department.Name,
                Description = department.Description,
                Floor = department.Floor,
                Users = department.Users.ToList().ConvertAll(u => UserResponseDto.Generate(u)) ?? new List<UserResponseDto>()
            };
        }
    }
}
