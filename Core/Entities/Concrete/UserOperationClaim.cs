using Core.Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Concrete
{
    public class UserOperationClaim : IEntity
    {
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int OperationClaimId { get; set; }
        [ForeignKey("OperationClaimId")]
        public OperationClaim OperationClaim { get; set; }

    }
}
