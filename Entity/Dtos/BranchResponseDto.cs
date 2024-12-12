using Core.Entities.Abstract;
using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class BranchResponseDto : IDto
    {
        public int BranchId { get; set; }
        public int MunicipalityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<DepartmentResponseDto> Departments { get; set; }

        public static BranchResponseDto Generate(Branch branch)
        {
            return new BranchResponseDto
            {
                BranchId = branch.Id,
                Description = branch.Description,
                MunicipalityId = branch.MunicipalityId,
                Name = branch.Name,
               // Departments = branch.Departments.ToList(),
                Departments = branch.Departments.ToList().ConvertAll(d => DepartmentResponseDto.Generate(d))
            };
        }
    }
}
