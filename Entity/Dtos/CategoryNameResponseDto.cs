using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class CategoryNameResponseDto
    {

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }


        public static CategoryNameResponseDto Generate(Category category)
        {
            return new CategoryNameResponseDto { CategoryId = category.Id, CategoryName=category.CategoryName };
        }
    }
}
