using Core.Entities.Abstract;
using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class OperationClaimResponseDto : IDto
    {
        public int Id { get; set; }

        public string OperationClaimName { get; set; }


        public static OperationClaimResponseDto Generate(OperationClaim operationClaim)
        {
            return new OperationClaimResponseDto
            {
                Id = operationClaim.Id,
                OperationClaimName = operationClaim.Name
            };
        }
    }
}
