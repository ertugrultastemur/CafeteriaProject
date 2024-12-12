using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class MunicipalityResponseDto
    {
        public int MunicipalityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<BranchResponseDto> Branches { get; set; }

        public static MunicipalityResponseDto Generate(Municipality municipality)
        {
            return new MunicipalityResponseDto
            {
                MunicipalityId = municipality.Id,
                Name = municipality.Name,
                Description = municipality.Description,
                Branches = municipality.Branches != null ? municipality.Branches.ToList().ConvertAll(b=>BranchResponseDto.Generate(b)) : new List<BranchResponseDto>()
            };
        }


    }
}
