using Core.Entities.Abstract;
using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Concrete
{
    public class Branch : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MunicipalityId { get; set; }
        [ForeignKey("MunicipalityId")]
        public virtual Municipality Municipality { get; set; }
        public virtual ICollection<Department> Departments { get; } = new List<Department>();
        public bool IsDeleted { get; set; }
    }
}
