using Core.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Concrete
{
    public class Category : IEntity
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<Product> Products { get; } = new List<Product>();
    }
}
