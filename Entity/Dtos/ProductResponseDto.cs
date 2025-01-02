using Core.Entities.Abstract;
using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class ProductResponseDto : IDto
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal UnitPrice { get; set; }
        public string ImagePath { get; set; }
        public int BranchId { get; set; }
        public bool IsDeleted { get; set; }


        public static ProductResponseDto Generate(Product product)
        {
            return new ProductResponseDto
            {
                ProductId = product.Id,
                CategoryId = product.Category.Id,
                CategoryName = product.Category.CategoryName ?? null,
                ProductName = product.Name,
                ProductDescription = product.Description,
                UnitPrice = product.UnitPrice,
                ImagePath = string.Join("\\", product.ImagePath.Split("\\").SkipWhile(part => part != "products")),
                BranchId = product.BranchId,
                IsDeleted = product.IsDeleted

            };

        }
    }
}
