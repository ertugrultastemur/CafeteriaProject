using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.Utilities;
using Core.Aspects.Autofac.Caching.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Log4Net;
using Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;
using Core.Dtos;
using Core.Entities.Concrete;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entity.Concrete;
using Entity.Dtos;
using log4net.Core;
using Microsoft.AspNetCore.Http;
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

        IBranchService _branchService;

        public ProductManager(IProductDal productDal, ICategoryService categoryService, IBranchService branchService)
        {
            _productDal = productDal;
            _categoryService = categoryService;
            _branchService = branchService;
        }

        [LogAspect(typeof(DatabaseLogger))]
        [SecuredOperation("product.add,admin")]
        //[ValidationAspect(typeof(ProductValidator))]
        [TransactionalOperation]
        [CacheRemoveAspect("IProductService.Get")]
        [CacheRemoveAspect("ICategoryService.Get")]
        public IResult Add(ProductRequestDto entity, IFormFile image)
        {
            UserDetailDto _user = UserDetailGetter.GetDetails();
            Branch branch = _branchService.GetByUserId(_user.Id).Data;
            List<IResult> result = BusinessRules.Check(CheckIfCategoryNotFound(entity.CategoryId),
                CheckIfProductCountOfCategoryCorrect(entity.CategoryId),
                CheckIfProductNameExistsOfProductsCorrect(branch.Id, entity.ProductName),
                CheckIfCategoryCountExcededThanLimitCouldNotAddingProductsCorrect()
                );
            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Product product = new Product();
            product.Name = entity.ProductName;
            product.Description = entity.ProductDescription;
            product.Category = _categoryService.GetAllByIds(new List<int> { entity.CategoryId }).Data[0];
            product.UnitPrice = entity.UnitPrice;
            product.ImagePath = ImageSaver.SaveImage(image).Result;
            product.Branch = branch;
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
        [LogAspect(typeof(DatabaseLogger))]
        public IDataResult<List<ProductResponseDto>> GetAll()
        {
            UserDetailDto _user = UserDetailGetter.GetDetails();
            Branch branch = _branchService.GetByUserId(_user.Id).Data;

            return new SuccessDataResult<List<ProductResponseDto>>(_productDal.GetAllAndDepends(p => p.Branch.Id == branch.Id, includeProperties:"Category,Orders").ConvertAll(p => ProductResponseDto.Generate(p)), Messages.ProductsListed);
        }

        [TransactionalOperation]
        [CacheRemoveAspect("IProductService.Get")]
        public IDataResult<ProductResponseDto> Update(ProductRequestDto productRequestDto, IFormFile image)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<ProductResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Category category = _categoryService.GetAllByIds(new List<int> { productRequestDto.CategoryId }).Data[0];
            Product product = _productDal.Get(p => p.Id == productRequestDto.ProductId);
            product.Name = productRequestDto.ProductName;
            product.Category = category;
            product.UnitPrice = productRequestDto.UnitPrice;
            product.Description = productRequestDto.ProductDescription;
            product.ImagePath = ImageSaver.SaveImage(image).Result;
            _productDal.Update(product);
            return new SuccessDataResult<ProductResponseDto>(ProductResponseDto.Generate(product), Messages.ProductUpdated);
        }

        public IDataResult<List<ProductResponseDto>> GetAllByCategoryId(int id)
        {
            UserDetailDto _user = UserDetailGetter.GetDetails();
            Branch branch = _branchService.GetByUserId(_user.Id).Data;

            return new SuccessDataResult<List<ProductResponseDto>>(_productDal.GetAll(p => p.Branch.Id ==branch.Id  && p.Category.Id == id).ConvertAll(p => ProductResponseDto.Generate(p)), Messages.ProductsListedByCategory);
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
            return new SuccessDataResult<List<Product>>(_productDal.GetAllAndDepends(includeProperties: "Category,Orders").FindAll(p => ids.Contains(p.Id)), Messages.ProductListed);
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

        private IResult CheckIfProductNameExistsOfProductsCorrect(int branchId, string productName)
        {
            if (_productDal.GetAll(p => p.Branch.Id == branchId && p.Name == productName).Any())
            {
                return new ErrorResult(Messages.ProductNameExistsOfProductsError);
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
