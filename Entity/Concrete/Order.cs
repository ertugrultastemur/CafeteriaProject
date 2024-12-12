using Core.Entities.Abstract;
using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Concrete
{
    public class Order : IEntity
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }

        public int UserId { get; set; }

        public virtual ICollection<OrderProduct> Products { get; } = new List<OrderProduct>();

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public bool Status { get; set; }

        public bool IsDeleted { get; set; }

    }
}
