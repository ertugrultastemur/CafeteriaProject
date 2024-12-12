using Core.Dtos;
using Core.Entities.Abstract;
using Core.Entities.Concrete;
using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class BranchRequestDto : IDto
    {
        public int BranchId { get; set; }
        public int MunicipalityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        public static BranchRequestDto Generate(Branch branch)
        {
            return new BranchRequestDto
            {
                BranchId=branch.Id,
                Description=branch.Description,
                MunicipalityId=branch.MunicipalityId,
                Name=branch.Name
            };
        }

        public static Branch Generate(BranchRequestDto branchRequestDto)
        {
            return new Branch
            {
                Id = branchRequestDto.BranchId,
                Name = branchRequestDto.Name,
                Description = branchRequestDto.Description,
                IsDeleted =false
            };
        }
    }
}
