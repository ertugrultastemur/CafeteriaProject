using Core.Entities.Abstract;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Concrete
{
    public class OperationClaim : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
            
        public ICollection<UserOperationClaim> Users { get; } = new List<UserOperationClaim>();

        public bool IsDeleted { get; set; }
    }
}
