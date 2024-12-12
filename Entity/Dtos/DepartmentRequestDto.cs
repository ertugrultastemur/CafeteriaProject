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
    public class DepartmentRequestDto : IDto
    {
        public int DepartmentId { get; set; }
        public int BranchId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Floor { get; set; }
        public List<int> UserIds { get; set; }


        public static DepartmentRequestDto Generate(Department department)
        {
            return new DepartmentRequestDto
            {
                DepartmentId = department.Id,
                BranchId = department.Branch.Id,
                Name = department.Name,
                Description = department.Description,
                Floor = department.Floor,
                UserIds = department.Users.Select(x => x.Id).ToList()
            };
        }
    }
}
