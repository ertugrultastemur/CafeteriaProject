using Core.Entities.Abstract;
using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class MunicipalityRequestDto : IDto
    {
        public int MunicipalityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<int> BranchIds { get; set; } = new List<int>();

        public static MunicipalityRequestDto Generate(Municipality municipality)
        {
            return new MunicipalityRequestDto
            {
                MunicipalityId=municipality.Id,
                Name=municipality.Name,
                Description=municipality.Description,
                BranchIds=municipality.Branches.Select(x => x.Id).ToList()
            };
        }


    }
}
