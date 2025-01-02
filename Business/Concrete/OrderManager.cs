using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.Utilities;
using Core.Aspects.Autofac.Caching.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;
using Core.Dtos;
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

        IOrderProductDal _orderProductDal;

        IProductService _productService;

        IUserService _userService;


        public OrderManager(IOrderDal orderDal, IProductService productService, IUserService userService, IOrderProductDal orderProductDal)
        {
            _orderDal = orderDal;
            _productService = productService;
            _userService = userService;
            _orderProductDal = orderProductDal;
        }

        [LogAspect(typeof(DatabaseLogger))]
        [CacheRemoveAspect("IOrderService.Get")]
        [TransactionalOperation]
        public IResult Add(OrderRequestDto orderRequestDto)
        {

            var result = BusinessRules.Check(CheckIfOrderProductsCount(orderRequestDto));

            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Order newOrder = new Order()
            {
                OrderDate = DateTime.Now,
                TotalPrice = 0,
                Status = 0,
                User = _userService.GetAllByIds(new List<int> { orderRequestDto.UserId }).Data[0],
                IsDeleted = false
            };
            if(orderRequestDto.ProductIds.Count != 0)
            {
                Order order = _orderDal.AddAndReturn(newOrder);
                List<Product> products = _productService.GetAllByIds(orderRequestDto.ProductIds).Data;
                List<OrderProduct> saltProducts = new List<OrderProduct>();
                foreach (var product in products)
                {
                    var existingProduct = saltProducts.FirstOrDefault(x => x.ProductId == product.Id);
                    if (existingProduct != null)
                    {
                        existingProduct.ProductQuantity++;
                        order.TotalPrice += existingProduct.Product.UnitPrice * existingProduct.ProductQuantity;
                    }
                    else
                    {
                        OrderProduct newProduct = new OrderProduct
                        {
                            Order = order,
                            Product = product,
                            ProductQuantity = orderRequestDto.ProductIds.Count(pId => pId.Equals(product.Id))
                        };
                        newProduct = _orderProductDal.AddAndReturn(newProduct);
                        saltProducts.Add(newProduct);
                        order.Products.Add(newProduct);
                        order.TotalPrice += product.UnitPrice * newProduct.ProductQuantity;
                    }
                }
                _orderDal.Update(order);
                return new SuccessResult(Messages.OrderAdded);
            }
            _orderDal.Add(newOrder);
            return new SuccessResult(Messages.OrderAdded);
        }

        [LogAspect(typeof(DatabaseLogger))]
        [CacheRemoveAspect("IOrderService.Get")]
        [TransactionalOperation]
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
            List<Order> orders = _orderDal.GetAllAndDepends(includeProperties: "Products,Products.Product,Products.Product.Category,User");
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
            return new SuccessDataResult<List<Order>>(_orderDal.GetAllAndDepends(includeProperties: "Products,User").FindAll(o => ids.Contains(o.Id)), Messages.OrdersListed);

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

        [LogAspect(typeof(DatabaseLogger))]
        [CacheRemoveAspect("IOrderService.Get")]
        [TransactionalOperation]
        public IDataResult<OrderResponseDto> Update(OrderRequestDto orderRequestDto)
        {
            var result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<OrderResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Order order = _orderDal.Get(o => o.Id.Equals(orderRequestDto.OrderId), includeProperties: "Products,Products.Product,Products.Product.Category,User");
            order.OrderDate = DateTime.Now;
            order.Products.ToList().ForEach(op => _orderProductDal.Delete(op));
            order.Products.Clear();
            if (orderRequestDto.ProductIds.Count != 0)
            {
                List<Product> products = _productService.GetAllByIds(orderRequestDto.ProductIds).Data;
                List<OrderProduct> saltProducts = new List<OrderProduct>();
                foreach (var product in products)
                {
                    var existingProduct = saltProducts.FirstOrDefault(x => x.ProductId == product.Id);
                    if (existingProduct != null)
                    {
                        existingProduct.ProductQuantity++;
                        order.TotalPrice += existingProduct.Product.UnitPrice * existingProduct.ProductQuantity;
                    }
                    else
                    {
                        OrderProduct newProduct = new OrderProduct
                        {
                            Order = order,
                            Product = product,
                            ProductQuantity = orderRequestDto.ProductIds.Count(pId => pId.Equals(product.Id))
                        };
                        newProduct = _orderProductDal.AddAndReturn(newProduct);
                        saltProducts.Add(newProduct);
                        order.Products.Add(newProduct);
                        order.TotalPrice += product.UnitPrice * newProduct.ProductQuantity;
                    }

                }
            }

            order.Status = orderRequestDto.Status;
            _userService.UpdateBalance(order.TotalPrice);
            _orderDal.Update(order);
            return new SuccessDataResult<OrderResponseDto>(OrderResponseDto.Generate(order), Messages.OrderUpdated);

        }

        [LogAspect(typeof(DatabaseLogger))]
        [CacheRemoveAspect("IOrderService.Get")]
        [TransactionalOperation]
        public IDataResult<OrderResponseDto> UpdateStatus(OrderRequestDto orderRequestDto)
        {
            var result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<OrderResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Order order = _orderDal.Get(o => o.Id.Equals(orderRequestDto.OrderId), includeProperties: "Products,Products.Product,Products.Product.Category,User");
            order.OrderDate = DateTime.Now;
            order.Status = orderRequestDto.Status;
            _userService.UpdateBalance(order.TotalPrice);
            _orderDal.Update(order);
            return new SuccessDataResult<OrderResponseDto>(OrderResponseDto.Generate(order), Messages.OrderUpdated);

        }

        [LogAspect(typeof(DatabaseLogger))]
        [CacheRemoveAspect("IOrderService.Get")]
        [TransactionalOperation]
        public IDataResult<OrderResponseDto> AddManualOrder(OrderRequestDto orderRequestDto)
        {
            var result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<OrderResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }

            Order order = new Order
            {
                OrderDate = DateTime.Now,
                TotalPrice = 0,
                Status = 1,
                User = _userService.GetAllByIds(new List<int> { orderRequestDto.UserId }).Data[0],
                IsDeleted = false
            };
            order.TotalPrice = orderRequestDto.TotalPrice;
            _userService.UpdateBalanceByUserId(orderRequestDto.UserId, order.TotalPrice);
            _orderDal.Add(order);
            return new SuccessDataResult<OrderResponseDto>(Messages.OrderAdded);
        }


        private IResult CheckIfOrderProductsCount(OrderRequestDto orderRequestDto)
        {
            if(orderRequestDto.ProductIds.Count == 0)
            {
                return new ErrorResult(Messages.OrderProductsCountError);

            }
            return new SuccessResult();
        }
    }
}
