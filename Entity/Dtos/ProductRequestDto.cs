using Core.Entities.Abstract;
using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class ProductRequestDto : IDto
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public short UnitsInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public List<int> OrderIds { get; set; }



        public static ProductRequestDto Generate(Product product)
        {
            return new ProductRequestDto
            {
                ProductId = product.Id,
                CategoryId = product.Category.Id,
                ProductName = product.Name,
                ProductDescription = product.Description,
                UnitsInStock = product.UnitsInStock,
                UnitPrice = product.UnitPrice,
                OrderIds = product.Orders.Select(x => x.OrderId).ToList(),

            };
        }

        public static Product Generate(ProductRequestDto productRequestDto)
        {
            return new Product
            {
                Id = productRequestDto.ProductId,
                Description = productRequestDto.ProductDescription,
                Name = productRequestDto.ProductName,
                UnitPrice = productRequestDto.UnitPrice,
                UnitsInStock= productRequestDto.UnitsInStock,
                IsDeleted = false
            };
        }
    }
}
