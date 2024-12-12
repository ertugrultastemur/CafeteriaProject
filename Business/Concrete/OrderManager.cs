using Business.Abstract;
using Business.Constants;
using Business.Utilities;
using Core.Aspects.Autofac.Caching.Caching;
using Core.Entities.Concrete;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entity.Concrete;
using Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class OrderManager : IOrderService
    {

        IOrderDal _orderDal;

        IProductService _productService;

        IUserService _userService;

        public OrderManager(IOrderDal orderDal, IProductService productService, IUserService userService)
        {
            _orderDal = orderDal;
            _productService = productService;
            _userService = userService;
        }

        [CacheRemoveAspect("IOrderService.Get")]
        public IResult Add(OrderRequestDto orderRequestDto)
        {
            var result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Order order = new Order()
            {
                OrderDate = DateTime.Now,
                TotalPrice = 0,
                User = _userService.GetAllByIds(new List<int> { orderRequestDto.UserId }).Data[0] ,
                IsDeleted = false
            };
            List<Product> products = _productService.GetAllByIds(orderRequestDto.ProductIds).Data;
            List<OrderProduct> saltProducts = new List<OrderProduct>();
            products.ForEach(product =>
            {
                var existingProduct = saltProducts.FirstOrDefault(x => x.ProductId == product.Id);
                if (existingProduct != null)
                {
                    existingProduct.ProductQuantity++;
                }
                else
                {
                    OrderProduct newProduct = new OrderProduct
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        ProductQuantity = orderRequestDto.ProductIds.Count(pId => pId.Equals(product.Id))
                    };
                    saltProducts.Add(newProduct);
                }
                order.TotalPrice += product.UnitPrice;

            });
            saltProducts.ForEach(p => { order.Products.Add(p); });
            _orderDal.Add(order);
            return new SuccessResult(Messages.OrderAdded);
        }

        [CacheRemoveAspect("IOrderService.Get")]
        public IResult Delete(int id)
        {
            {
                List<IResult> result = BusinessRules.Check();

                if (result.Count != 0)
                {
                    return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
                }
                Order order = _orderDal.Get(o => o.Id.Equals(id));
                order.IsDeleted = true;
                _orderDal.Update(order);

                return new SuccessResult(Messages.OrderDeleted);
            }
        }

        [CacheAspect]
        public IDataResult<List<OrderResponseDto>> GetAll()
        {
            
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<List<OrderResponseDto>>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            List<Order> orders = _orderDal.GetAllAndDepends(includeProperties: "Products.Product,User");
            return new SuccessDataResult<List<OrderResponseDto>>(orders
                .ConvertAll(o => OrderResponseDto.Generate(o)), Messages.OrdersListed);
            
        }

        public IDataResult<List<Order>> GetAllByIds(List<int> ids)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<List<Order>>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<List<Order>>(_orderDal.GetAll().FindAll(o => ids.Contains(o.Id)), Messages.OrdersListed);

        }

        public IDataResult<OrderResponseDto> GetById(int orderId)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<OrderResponseDto>( result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<OrderResponseDto>(OrderResponseDto.Generate(_orderDal.Get(o => o.Id.Equals(orderId))), Messages.OrderListed);
        }

        [CacheRemoveAspect("IOrderService.Get")]
        public IDataResult<OrderResponseDto> Update(OrderRequestDto orderRequestDto)
        {
            var result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<OrderResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Order order = _orderDal.Get(o => o.Id.Equals(orderRequestDto.OrderId));
            order.OrderDate = orderRequestDto.OrderDate;
            List<Product> products = _productService.GetAllByIds(orderRequestDto.ProductIds).Data;
            order.Products.Clear();

            products.Select(product => new OrderProduct
            {
                OrderId = order.Id,
                ProductId = product.Id,
                ProductQuantity = orderRequestDto.ProductIds.Count(pId => pId.Equals(product.Id))
            }).ToList().ForEach(p => order.Products.Add(p));

            _orderDal.Update(order);
            return new SuccessDataResult<OrderResponseDto>(Messages.OrderUpdated);
        }
    }
}
