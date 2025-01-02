using Core.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class OperationClaimDto : IDto
    {
        public List<int>? OperationClaimIds { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }
    }
}
