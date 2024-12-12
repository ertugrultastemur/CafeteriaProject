using Core.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Concrete
{
    public class Product : IEntity
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public short UnitsInStock { get; set; }

        public decimal UnitPrice { get; set; }

        public virtual ICollection<OrderProduct> Orders { get; set; } = new List<OrderProduct>();

        public bool IsDeleted { get; set; }
    }
}
