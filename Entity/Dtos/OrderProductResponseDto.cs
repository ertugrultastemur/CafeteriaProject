using Core.Entities.Abstract;
using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class OrderProductResponseDto : IDto
    {
        public ProductResponseDto Product { get; set; }

        public int Quantity { get; set; }


        public static OrderProductResponseDto Generate(OrderProduct orderProduct)
        {
            return new OrderProductResponseDto
            {
                Product = orderProduct.Product != null ? ProductResponseDto.Generate(orderProduct.Product) : new ProductResponseDto(),
                Quantity = orderProduct.ProductQuantity,
            };
        }
    }
}
