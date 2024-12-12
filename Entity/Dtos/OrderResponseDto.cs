﻿using Core.Entities.Abstract;
using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class OrderResponseDto : IDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<int> ProductIds { get; set; }
        public int UserId { get; set; }

        public static OrderResponseDto Generate(Order order)
        {
            return new OrderResponseDto
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                ProductIds = order.Products != null ? order.Products.Select(p => p.ProductId).ToList() : new List<int>(),
                UserId = order.UserId,
            };
        }
    }
}