using Core.Dtos;
using Core.Entities.Abstract;
using Core.Entities.Concrete;
using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class CategoryResponseDto : IDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public List<ProductResponseDto> Products { get; set; }

        public static CategoryResponseDto Generate(Category category)
        {
            return new CategoryResponseDto
            {
                CategoryId = category.Id,
                CategoryName = category.CategoryName,
                Products = category.Products.ToList().ConvertAll(p => ProductResponseDto.Generate(p)),
            };
        }
    }
}
