using Core.Entities.Abstract;
using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class OrderRequestDto : IDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<int> ProductIds { get; set; }
        public int UserId { get; set; }

        public static OrderRequestDto Generate(Order order)
        {
            return new OrderRequestDto
            {
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                ProductIds = order.Products.Select(p => p.ProductId).ToList(),
                UserId = order.User.Id,
            };
        }
    }
}
