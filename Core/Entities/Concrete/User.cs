using Core.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Entities.Concrete
{
    public class User : IEntity
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public Byte[] PasswordHash { get; set; }

        public Byte[] PasswordSalt { get; set; }

        public ICollection<UserOperationClaim> OperationClaims { get; set; } = new List<UserOperationClaim>();

        public int DepartmentId { get; set; }

        public decimal Balance { get; set; }

        public bool IsDeleted { get; set; }
    }
}
