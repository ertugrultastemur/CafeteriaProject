using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.Utilities;
using Core.Aspects.Autofac.Caching.Caching;
using Core.Dtos;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entity.Concrete;
using Entity.Dtos;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CategoryManager : ICategoryService
    {
        ICategoryDal _categoryDal;

        IBranchService _branchService;

        public CategoryManager(ICategoryDal categoryDal, IBranchService branchService)
        {
            _categoryDal = categoryDal;
            _branchService = branchService;
        }

        [TransactionalOperation]
        [CacheRemoveAspect("ICategoryService.Get")]
        public IResult Add(CategoryRequestDto category)
        {
            var result = BusinessRules.Check(CheckIfCategoryNameNullOrExists(category.CategoryName));

            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Category _category = new Category() { CategoryName=category.CategoryName, IsDeleted=false};
            _categoryDal.Add(_category);
            return new SuccessResult(Messages.CategoryAdded);
        }

        [TransactionalOperation]
        [CacheRemoveAspect("ICategoryService.Get")]
        public IDataResult<CategoryResponseDto> Update(CategoryRequestDto categoryRequestDto)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<CategoryResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Category category = _categoryDal.Get(c => c.Id == categoryRequestDto.CategoryId);
            category.CategoryName = categoryRequestDto.CategoryName;
            _categoryDal.Update(category);
            return new SuccessDataResult<CategoryResponseDto>(CategoryResponseDto.Generate(category), Messages.CategoryUpdated);
        }

        [TransactionalOperation]
        [CacheRemoveAspect("ICategoryService.Get")]
        public IResult Delete(int id)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Category category = _categoryDal.Get(c => c.Id.Equals(id));
            category.IsDeleted = true;
            _categoryDal.Update(category);

            return new SuccessResult(Messages.UserAdded);
        }

        [CacheAspect]
        public IDataResult<List<CategoryResponseDto>> GetAll()
        {
            UserDetailDto _user = UserDetailGetter.GetDetails();
            Branch branch = _branchService.GetByUserId(_user.Id).Data;
            return new SuccessDataResult<List<CategoryResponseDto>>(_categoryDal.GetAll().ConvertAll(c => CategoryResponseDto.Generate(c)), Messages.CategoriesListed);

        }

        [CacheAspect]
        public IDataResult<List<Category>> GetAllByIds(List<int> ids)
        {
            return new SuccessDataResult<List<Category>>(_categoryDal.GetAllAndDepends(includeProperties: "Products").FindAll(c => ids.Contains(c.Id)));

        }

        [CacheAspect]
        public IDataResult<List<CategoryNameResponseDto>> GetAllCategoryNames()
        {
            return new SuccessDataResult<List<CategoryNameResponseDto>>(_categoryDal.GetAll().ConvertAll(c => CategoryNameResponseDto.Generate(c)), Messages.CategoriesListed);

        }

        public IDataResult<CategoryResponseDto> GetById(int categoryId)
        {
            return new SuccessDataResult<CategoryResponseDto>(CategoryResponseDto.Generate(_categoryDal.Get(c => c.Id == categoryId)), Messages.CategoryListed);
        }

        public IResult CheckIfCategoryNameNullOrExists(string categoryName)
        {
            if (categoryName == null || _categoryDal.GetAll(c => c.CategoryName == categoryName).Any())
            {
                return new ErrorResult(Messages.CategoryNullOrExists);
            }
            return new SuccessResult();
        }

        public IResult CheckIfCategoryIdNotExists(int id)
        {
            if (id == null || !_categoryDal.GetAll(c => c.Id == id).Any())
            {
                return new ErrorResult(Messages.CategoryNotExists);
            }
            return new SuccessResult();
        }
    }
}
