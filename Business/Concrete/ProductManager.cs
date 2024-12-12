using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Core.Aspects.Autofac.Caching.Caching;
using Core.Aspects.Autofac.Validation;
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
    public class ProductManager : IProductService
    {
        IProductDal _productDal;

        ICategoryService _categoryService;



        public ProductManager(IProductDal productDal, ICategoryService categoryService)
        {
            _productDal = productDal;
            _categoryService = categoryService;
        }

        [SecuredOperation("product.add,admin")]
        //[ValidationAspect(typeof(ProductValidator))]
        [TransactionalOperation]
        [CacheRemoveAspect("IProductService.Get")]
        public IResult Add(ProductRequestDto entity)
        {
            List<IResult> result = BusinessRules.Check(CheckIfCategoryNotFound(entity.CategoryId),
                CheckIfProductCountOfCategoryCorrect(entity.CategoryId),
                CheckIfProductNameExistsOfProductsCorrect(entity.ProductName),
                CheckIfCategoryCountExcededThanLimitCouldNotAddingProductsCorrect()
                );
            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }

            Product product = new Product();
            product.Name = entity.ProductName;
            product.Description = entity.ProductDescription;
            product.CategoryId = entity.CategoryId;
            product.UnitsInStock = entity.UnitsInStock;
            product.UnitPrice = entity.UnitPrice;
            product.IsDeleted = false;
            _productDal.Add(product);
            return new SuccessResult(Messages.ProductAdded);
        }

        [TransactionalOperation]
        [CacheRemoveAspect("IProductService.Get")]
        public IResult Delete(int id)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Product product = _productDal.Get(u => u.Id == id);
            product.IsDeleted = true;
            _productDal.Update(product);

            return new SuccessResult(Messages.ProductDeleted);
        }
        [CacheAspect]
        public IDataResult<List<ProductResponseDto>> GetAll()
        {
            return new SuccessDataResult<List<ProductResponseDto>>(_productDal.GetAllAndDepends(includeProperties:"Category,Orders").ConvertAll(p => ProductResponseDto.Generate(p)), Messages.ProductsListed);
        }

        [TransactionalOperation]
        [CacheRemoveAspect("IProductService.Get")]
        public IDataResult<ProductResponseDto> Update(ProductRequestDto productRequestDto)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<ProductResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Product product = _productDal.Get(p => p.Id == productRequestDto.ProductId);
            product.Name = productRequestDto.ProductName;
            product.Description = productRequestDto.ProductDescription;
            product.Orders.Clear();
            /*List<Order> orders = _orderService.GetAllByIds(productRequestDto.OrderIds).Data;
            orders.Select(order => new OrderProduct
            {
                OrderId = order.Id,
                ProductId = product.Id,
                ProductQuantity = productRequestDto.OrderIds.Count(oId => oId.Equals(order.Id))
            }).ToList().ForEach(o => product.Orders.Add(o));*/
            _productDal.Update(product);
            return new SuccessDataResult<ProductResponseDto>(ProductResponseDto.Generate(product), Messages.ProductUpdated);
        }

        public IDataResult<List<ProductResponseDto>> GetAllByCategoryId(int id)
        {
            return new SuccessDataResult<List<ProductResponseDto>>(_productDal.GetAll(p => p.Category.Id == id).ConvertAll(p => ProductResponseDto.Generate(p)), Messages.ProductsListedByCategory);
        }

        public IDataResult<List<ProductDetailDto>> GetProductDetails()
        {
            if (DateTime.Now.Hour == 19)
            {
                return new ErrorDataResult<List<ProductDetailDto>>(Messages.MaintanceTime);
            }
            return new SuccessDataResult<List<ProductDetailDto>>(_productDal.GetProductDetails(), Messages.ProductDetailsListed);
        }

        public IDataResult<List<ProductResponseDto>> GetByUnitPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<ProductResponseDto>>(_productDal.GetAll(p => p.UnitPrice >= min && p.UnitPrice <= max).ConvertAll(p => ProductResponseDto.Generate(p)), Messages.ProductsListedByUnitPrice);
        }

        [CacheAspect]
        public IDataResult<ProductResponseDto> GetById(int productId)
        {
            List<IResult> results = BusinessRules.Check(CheckIfProductNotFound(productId));

            if (results.Count != 0)
            {
                return new ErrorDataResult<ProductResponseDto>(results.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<ProductResponseDto>(ProductResponseDto.Generate(_productDal.Get(p => p.Id == productId)), Messages.ProductListed);
        }

        public IDataResult<List<Product>> GetAllByIds(List<int> ids)
        {
            List<IResult> results = BusinessRules.Check();

            if (results.Count != 0)
            {
                return new ErrorDataResult<List<Product>>(results.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<List<Product>>(_productDal.GetAll().FindAll(p => ids.Contains(p.Id)), Messages.ProductListed);
        }

        private IResult CheckIfCategoryNotFound(int categoryId)
        {
            if (_categoryService.GetById(categoryId).Data == null)
            {
                return new ErrorResult(Messages.CategoryNotFound);
            }
            return new SuccessResult();
        }

        private IResult CheckIfProductNotFound(int productId)
        {
            if (!_productDal.GetAll(p => p.Id == productId).Any())
            {
                return new ErrorResult(Messages.ProductNotFound);
            }
            return new SuccessResult();
        }

        private IResult CheckIfProductNameExistsOfProductsCorrect(string productName)
        {
            if (_productDal.GetAll(p => p.Name == productName).Any())
            {
                return new ErrorResult(Messages.ProductNameExistsOfProducctsError);
            }
            return new SuccessResult();
        }

        private IResult CheckIfProductCountOfCategoryCorrect(int categoryId)
        {
            int count = _productDal.GetAll(p => p.Category.Id == categoryId).Count();
            if (count >= 15) return new ErrorResult(Messages.ProductCountOfCategoryError);
            return new SuccessResult();
        }

        private IResult CheckIfCategoryCountExcededThanLimitCouldNotAddingProductsCorrect()
        {

            if (_categoryService.GetAll().Data.Count < 15)
            {
                return new SuccessResult();
            }
            return new ErrorResult(Messages.CategoryCountExcededThanLimitCouldNotAddingProductsError);
        }
    }
}
